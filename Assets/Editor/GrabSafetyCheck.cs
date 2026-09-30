using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrabSafetyTestHand : XRBaseInteractor
{
    public override bool isSelectActive => true;
    public override void GetValidTargets(List<IXRInteractable> targets)
    {
        targets.Clear();
        foreach(var item in interactablesSelected)targets.Add(item);
    }
}

[InitializeOnLoad]
public static class GrabSafetyCheck
{
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public;
    static readonly StringBuilder report=new StringBuilder();
    static int step;static double nextTick;
    static XRInteractionManager manager;static GrabSafetyTestHand hand;
    static XRGrabInteractable pills,core;static SupplyBoxPuzzle box;static GeneratorCoreSocket port;
    static GameObject wall;static Vector3 pillHome;static XRGrabInteractable[] loose;
    static Exception failure;
    static object Field(object target,string name)=>target.GetType().GetField(name,Flags).GetValue(target);
    static T One<T>() where T:UnityEngine.Object=>UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include).Single();
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);report.AppendLine("PASS "+message);}
    static GrabSafetyCheck(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run()
    {
        EditorSceneManager.OpenScene(MP1cBuildTools.ScenePath);
        SessionState.SetBool("GrabSafetyActive",true);SessionState.SetBool("GrabSafetyPassed",false);
        EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state)
    {
        if(!SessionState.GetBool("GrabSafetyActive",false))return;
        if(state==PlayModeStateChange.EnteredPlayMode)
        {
            report.Clear();step=0;failure=null;nextTick=EditorApplication.timeSinceStartup+2;
            EditorApplication.update+=Tick;Application.logMessageReceived+=Log;
        }
        if(state==PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool("GrabSafetyActive",false);
            if(SessionState.GetBool("GrabSafetyPassed",false))EditorApplication.delayCall+=RoomIntegrationCheck.Run;
            else EditorApplication.Exit(1);
        }
    }
    static void Log(string message,string stack,LogType type)
    {
        if(type==LogType.Exception||(type==LogType.Error&&!message.Contains("OpenXR")&&!message.Contains("XR_ERROR")))failure=new Exception(message+"\n"+stack);
    }
    static void Pose(XRGrabInteractable item,Vector3 position)
    {
        var body=item.GetComponent<Rigidbody>();body.position=position;item.transform.position=position;
        if(!body.isKinematic){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
        Physics.SyncTransforms();
    }
    static void Hold(XRGrabInteractable item)
    {
        hand.transform.SetPositionAndRotation(item.transform.position,item.transform.rotation);
        manager.SelectEnter(hand,(IXRSelectInteractable)item);
        Check(item.isSelected,item.name+" is held through XR selection");
    }
    static void Tick()
    {
        if(EditorApplication.timeSinceStartup<nextTick)return;
        try
        {
            if(failure!=null)throw failure;
            if(step==0)
            {
                var simulator=GameObject.Find("XR Interaction Simulator");if(simulator)simulator.SetActive(false);
                manager=One<XRInteractionManager>();box=One<SupplyBoxPuzzle>();port=One<GeneratorCoreSocket>();
                pills=UnityEngine.Object.FindObjectsByType<PuzzleItem>().Single(i=>i.itemType==SupplyItemType.Pills).GetComponent<XRGrabInteractable>();
                core=One<GeneratorCoreItem>().GetComponent<XRGrabInteractable>();
                pillHome=(Vector3)Field(pills.GetComponent<DroppedItemRecovery>(),"startingPosition");
                var grabs=UnityEngine.Object.FindObjectsByType<XRGrabInteractable>(FindObjectsInactive.Include).Where(g=>!g.GetComponent<CollectibleItem>()).ToArray();
                Check(grabs.All(g=>g.GetComponent<DroppedItemRecovery>()),"Every loose scene prop has recovery");
                Check(grabs.All(g=>g.movementType==XRBaseInteractable.MovementType.VelocityTracking),"All loose scene props use physics while held");
                var collider=core.GetComponent<Collider>();
                port.SendMessage("OnTriggerStay",collider);
                Check(!(bool)Field(port,"completed")&&!core.enabled,"Locked generator core cannot be installed early");
                var handObject=new GameObject("GrabSafetyTestHand");hand=handObject.AddComponent<GrabSafetyTestHand>();hand.keepSelectedTargetValid=true;
                Pose(pills,new Vector3(0,5,0));Hold(pills);
                wall=GameObject.CreatePrimitive(PrimitiveType.Cube);wall.name="GrabSafetyTestWall";wall.transform.position=new Vector3(.5f,5,0);wall.transform.localScale=new Vector3(.1f,2,2);
                Physics.SyncTransforms();
            }
            else if(step==1)
            {
                Check(!pills.GetComponent<Rigidbody>().isKinematic,"Held pills remain a dynamic physics body");
                hand.transform.position=new Vector3(1.2f,5,0);
            }
            else if(step==2)
            {
                Check(pills.isSelected&&pills.GetComponent<Collider>().bounds.max.x<.49f,"Held pills stop at a wall while the controller moves through it");
                UnityEngine.Object.DestroyImmediate(wall);
                hand.transform.position=new Vector3(0,-4,0);Pose(pills,hand.transform.position);
            }
            else if(step==3)
            {
                Check(!pills.isSelected&&pills.enabled&&Vector3.Distance(pills.transform.position,pillHome)<.4f,"Held item pulled below the level releases and returns home");
                Check(pills.GetComponent<Rigidbody>().linearVelocity.magnitude<2,"Recovery does not replay the old throw velocity");
                var intake=box.transform.Find("PackingIntake").GetComponent<Collider>();
                Pose(pills,intake.bounds.center+Vector3.up*.4f);Hold(pills);
                hand.transform.position=intake.bounds.center;
            }
            else if(step==4)
            {
                Check(!pills.enabled&&!pills.isSelected&&pills.GetComponent<Rigidbody>().isKinematic,"Held pills latch through the actual box trigger");
                Check(((HashSet<SupplyItemType>)Field(box,"placedItems")).Count==1,"Pills count once after capture");
                hand.transform.position=new Vector3(0,5,0);
                One<WallCabinetDoor>().OpenDoor();
                var intake=port.GetComponent<Collider>().bounds.center;
                var centerOffset=core.GetComponent<Collider>().bounds.center-core.transform.position;
                Pose(core,intake-centerOffset+Vector3.up*.45f);Hold(core);
                hand.transform.position=intake-centerOffset;
            }
            else if(step==5)
            {
                Check((bool)Field(port,"completed")&&!core.enabled&&!core.isSelected&&core.GetComponent<Rigidbody>().isKinematic,"Held generator core latches through the actual port trigger");
                hand.transform.position=new Vector3(0,7,0);
                var recovery=UnityEngine.Object.FindObjectsByType<XRGrabInteractable>().Where(g=>g.enabled&&!g.GetComponent<CollectibleItem>()&&g.GetComponent<DroppedItemRecovery>()).ToArray();
                loose=recovery;
                foreach(var item in loose)Pose(item,new Vector3(item.transform.position.x,-4,item.transform.position.z));
            }
            else if(step==6)
            {
                foreach(var item in loose)Check(item.transform.position.y>-1.2f&&item.enabled,"Lost prop recovered: "+item.name);
                Check(!pills.enabled&&pills.GetComponent<Rigidbody>().isKinematic,"Packed pills stay secured after later controller movement");
                Check(!core.enabled&&Vector3.Distance(core.transform.position,port.snapPoint.position)<.01f,"Installed core stays seated after later controller movement");
                Check(((HashSet<SupplyItemType>)Field(box,"placedItems")).Count==1,"Recovery preserves packing progress");
                var remaining=UnityEngine.Object.FindObjectsByType<PuzzleItem>().Where(i=>i.itemType!=SupplyItemType.Pills);
                foreach(var item in remaining)box.TryPlaceItem(item);
                Check(((GameObject)Field(box,"rewardItem")).activeSelf,"Packing can still finish after item recovery");
                Finish(true);return;
            }
            step++;nextTick=EditorApplication.timeSinceStartup+1;
        }
        catch(Exception e){report.AppendLine("FAIL "+e);Debug.LogException(e);Finish(false);}
    }
    static void Finish(bool passed)
    {
        EditorApplication.update-=Tick;Application.logMessageReceived-=Log;
        File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"grab-safety-check.txt"),report.ToString());
        SessionState.SetBool("GrabSafetyPassed",passed);EditorApplication.isPlaying=false;
    }
}
