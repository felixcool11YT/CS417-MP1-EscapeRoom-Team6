using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[InitializeOnLoad]
public static class DepartureStationIntegrationCheck
{
    static double nextTick;static int step;static StringBuilder report=new StringBuilder();
    static DepartureStationController station;static FinalDoorController door;static Transform root;
    static XRGrabInteractable[] items;static XRSocketInteractor[] sockets;
    static BindingFlags flags=BindingFlags.NonPublic|BindingFlags.Instance;
    static DepartureStationIntegrationCheck(){EditorApplication.playModeStateChanged+=Changed;}
    static object Field(object target,string name)=>target.GetType().GetField(name,flags).GetValue(target);
    static void Set(object target,string name,object value)=>target.GetType().GetField(name,flags).SetValue(target,value);
    static void Assert(bool condition,string description){if(!condition)throw new Exception(description);report.AppendLine("PASS "+description);}
    static string Stage=>Field(station,"currentStage").ToString();
    static void Filter(int i)=>typeof(DepartureStationController).GetMethod("SelectFilter").Invoke(station,new object[]{i});
    public static void Begin()
    {
        var scene=UnityEngine.SceneManagement.SceneManager.GetActiveScene();foreach(var go in scene.GetRootGameObjects())if(go.name!="MP1C_DepartureStation"&&go.name!="FinalDoorSystem"&&go.GetComponent<XRInteractionManager>()==null)go.SetActive(false);
        // Isolate side effects; test the actual door authorization/unlock code.
        var door=UnityEngine.Object.FindAnyObjectByType<FinalDoorController>();Set(door,"doorAnimator",null);Set(door,"exitArea",null);Set(door,"winPanel",null);Set(door,"unlockParticles",null);
        var floor=GameObject.CreatePrimitive(PrimitiveType.Cube);floor.name="TemporaryTestFloor";floor.transform.position=new Vector3(2,-.1f,-9.4f);floor.transform.localScale=new Vector3(10,.2f,10);
        SessionState.SetBool("DepartureCheckActive",true);SessionState.SetBool("DepartureCheckPassed",false);EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state)
    {
        if(!SessionState.GetBool("DepartureCheckActive",false))return;
        if(state==PlayModeStateChange.EnteredPlayMode){step=0;nextTick=EditorApplication.timeSinceStartup+2;EditorApplication.update+=Tick;}
        if(state==PlayModeStateChange.EnteredEditMode){SessionState.SetBool("DepartureCheckActive",false);if(SessionState.GetBool("DepartureCheckPassed",false))EditorApplication.delayCall+=MP1cBuildTools.BuildQuest;else EditorApplication.Exit(1);}
    }
    static void Tick()
    {
        if(EditorApplication.timeSinceStartup<nextTick)return;
        try
        {
            if(step==0)
            {
                station=UnityEngine.Object.FindAnyObjectByType<DepartureStationController>();root=station.transform;door=UnityEngine.Object.FindAnyObjectByType<FinalDoorController>();items=(XRGrabInteractable[])Field(station,"supplyItems");sockets=(XRSocketInteractor[])Field(station,"supplySockets");
                Assert(Stage=="AwaitingAuthorization"&&!items[0].enabled&&!sockets[0].socketActive,"Station starts locked");Filter(0);Assert((int)Field(station,"filterStep")==0,"Filter input ignored before purge");door.CompleteDeparture();Assert(!(bool)Field(door,"isUnlocked"),"Door rejects completion before authorization");
                door.RegisterItem();door.RegisterItem();Assert(Stage=="AwaitingAuthorization","Two rewards do not authorize station");door.RegisterItem();Assert(Stage=="Packing"&&items[0].enabled&&sockets[0].socketActive,"Third reward enables packing");Assert((bool)Field(door,"isUnlocked"),"Third reward opens inner door");
                foreach(var pair in items.Zip(sockets,(item,socket)=>new{item,socket})){pair.item.transform.position+=pair.socket.attachTransform.position-pair.item.attachTransform.position+new Vector3(.025f,.015f,.02f);var rb=pair.item.GetComponent<Rigidbody>();rb.position=pair.item.transform.position;rb.linearVelocity=Vector3.zero;}Physics.SyncTransforms();step=1;nextTick=EditorApplication.timeSinceStartup+2;
            }
            else
            {
                Assert(Stage=="Purging","Packing both supplies advances to purge");Assert(items.All(i=>!i.enabled&&i.GetComponent<Rigidbody>().isKinematic),"Packed supplies are secured");Assert(items.Zip(sockets,(item,socket)=>Vector3.Distance(item.attachTransform.position,socket.attachTransform.position)).All(d=>d<.025f),"Supplies arriving off-center are seated at their anchors");
                Filter(2);Assert((int)Field(station,"filterStep")==0&&Stage=="Purging","Wrong first filter resets sequence");Filter(0);Assert((int)Field(station,"filterStep")==1,"First correct filter advances");Assert(((TMPro.TMP_Text)Field(station,"purgeText")).text=="CYCLES 1 / 3","Each correct filter updates the display");Filter(0);Assert((int)Field(station,"filterStep")==0,"Repeated filter resets sequence");Filter(0);Filter(1);Filter(2);Assert(Stage=="Powering","Particulate, chemical, radiation enables power");
                station.BeginPreparation();Assert(Stage=="Powering","Repeated authorization cannot reset progress");station.CheckPower();Assert(Stage=="Powering"&&!station.IsComplete,"Wrong breakers leave departure incomplete");var breakers=(BreakerSwitch[])Field(station,"breakers");breakers[0].SetState(true);breakers[1].SetState(true);breakers[2].SetState(false);station.CheckPower();Assert(Stage=="Complete"&&station.IsComplete,"Correct breaker test completes departure preparation");Filter(0);Assert(Stage=="Complete","Filter input cannot change completed stage");
                Finish(true);
            }
        }catch(Exception e){report.AppendLine("FAIL "+e);Debug.LogException(e);Finish(false);}
    }
    static void Finish(bool ok){File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"integration-check.txt"),report.ToString());SessionState.SetBool("DepartureCheckPassed",ok);EditorApplication.update-=Tick;EditorApplication.isPlaying=false;}
}
