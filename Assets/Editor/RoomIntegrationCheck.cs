using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

[InitializeOnLoad]
public static class RoomIntegrationCheck
{
    static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    static StringBuilder report = new StringBuilder();
    static int step;
    static double nextTick;
    static SupplyBoxPuzzle supply;
    static WallCabinetDoor cabinet;
    static ValveSequenceController valves;
    static TerminalCodePuzzle terminal;
    static PuzzleItem[] packed;
    static GameObject key;
    static Exception runtimeFailure;
    static RoomIntegrationCheck()
    {
        EditorApplication.playModeStateChanged += Changed;
        if(SessionState.GetBool("RoomCheckActive",false))Application.logMessageReceived += Log;
    }
    static object Field(object target, string name) => target.GetType().GetField(name, Flags).GetValue(target);
    static void Call(object target, string method, params object[] args) => target.GetType().GetMethod(method, Flags).Invoke(target, args);
    static T One<T>() where T : UnityEngine.Object => UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include).Single();
    static void Check(bool ok, string message) { if (!ok) throw new Exception(message); report.AppendLine("PASS " + message); }

    public static void Run()
    {
        try
        {
            report.Clear();
            var scene = EditorSceneManager.OpenScene(MP1cBuildTools.ScenePath);
            foreach (var go in scene.GetRootGameObjects())
            foreach (var t in go.GetComponentsInChildren<Transform>(true))
            {
                Check(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)==0, "Script references: " + SceneAudit.PathOf(t));
                foreach (var component in t.GetComponents<MonoBehaviour>())
                {
                    if (!component) continue;
                    var so = new SerializedObject(component); var p = so.GetIterator();
                    while (p.NextVisible(true))
                    {
                        if (!p.isArray || !p.propertyPath.EndsWith("m_PersistentCalls.m_Calls")) continue;
                        for (int i=0; i<p.arraySize; i++)
                        {
                            var listener=p.GetArrayElementAtIndex(i); var method=listener.FindPropertyRelative("m_MethodName").stringValue;
                            if (string.IsNullOrEmpty(method)) continue;
                            var target=listener.FindPropertyRelative("m_Target").objectReferenceValue;
                            Check(target && target.GetType().GetMethods(Flags).Any(m=>m.Name==method), "Event: " + SceneAudit.PathOf(t) + " -> " + method);
                        }
                    }
                }
            }
            foreach(var es in UnityEngine.Object.FindObjectsByType<UnityEngine.EventSystems.EventSystem>())
                Check(es.GetComponents<UnityEngine.EventSystems.BaseInputModule>().Length==1&&es.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(),"Gameplay uses one XR UI input module");
            var bodies=scene.GetRootGameObjects().SelectMany(g=>g.GetComponentsInChildren<Rigidbody>(true)).Where(b=>b.gameObject.activeInHierarchy&&!b.isKinematic).ToArray();
            Check(bodies.All(b=>b.mass>0)&&bodies.Max(b=>b.mass)/bodies.Min(b=>b.mass)<=20, "Dynamic mass ratio <= 20:1");
            Check(bodies.Count(b=>!Mathf.Approximately(b.mass,1))>bodies.Length/2, "Most movable bodies use non-default masses");
            report.AppendLine("MASS " + bodies.Length + " bodies; " + bodies.Min(b=>b.mass) + ".." + bodies.Max(b=>b.mass) + " kg");
            foreach(var go in scene.GetRootGameObjects()) if(go.name=="XR Interaction Simulator")go.SetActive(false);
            Directory.CreateDirectory(MP1cBuildTools.Output);
            File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"scene-integrity-check.txt"),report.ToString());
            report.Clear();
            SessionState.SetBool("RoomCheckActive",true); SessionState.SetBool("RoomCheckPassed",false);
            var start=EditorSceneManager.OpenScene("Assets/EscapeRoom/Scenes/Start.unity");
            foreach(var go in start.GetRootGameObjects())if(go.name=="XR Interaction Simulator")go.SetActive(false);
            EditorApplication.isPlaying=true;
        }
        catch(Exception e) { Debug.LogException(e); File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"scene-integrity-check.txt"),report+"FAIL "+e); EditorApplication.Exit(1); }
    }

    static void TestCollisions()
    {
        var testScene=SceneManager.CreateScene("MassCollisionCheck",new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        var physics=testScene.GetPhysicsScene();
        float[] speeds=new float[2];
        for(int i=0;i<2;i++)
        {
            float mass=i==0?.1f:1.5f;
            var target=GameObject.CreatePrimitive(PrimitiveType.Cube); var projectile=GameObject.CreatePrimitive(PrimitiveType.Cube);
            SceneManager.MoveGameObjectToScene(target,testScene); SceneManager.MoveGameObjectToScene(projectile,testScene);
            target.transform.position=new Vector3(0,0,i*5); projectile.transform.position=new Vector3(-1.3f,0,i*5);
            target.transform.localScale=projectile.transform.localScale=Vector3.one*.2f;
            var a=target.AddComponent<Rigidbody>(); a.useGravity=false; a.mass=mass; a.linearDamping=0;
            var b=projectile.AddComponent<Rigidbody>(); b.useGravity=false; b.mass=.5f; b.linearDamping=0; b.linearVelocity=Vector3.right*4;
            Physics.SyncTransforms();
            for(int frame=0;frame<60;frame++)physics.Simulate(1f/60);
            speeds[i]=a.linearVelocity.x;
            UnityEngine.Object.DestroyImmediate(target); UnityEngine.Object.DestroyImmediate(projectile);
        }
        Check(speeds[0]>speeds[1]&&speeds[1]>0,"Equal projectile impacts move the light body faster than the heavy body");
        report.AppendLine("COLLISION target speeds: " + speeds[0] + " m/s (0.1 kg), " + speeds[1] + " m/s (1.5 kg)");
        SceneManager.UnloadSceneAsync(testScene);
    }
    static void Changed(PlayModeStateChange state)
    {
        if(!SessionState.GetBool("RoomCheckActive",false))return;
        if(state==PlayModeStateChange.EnteredPlayMode)
        {
            step=-1; nextTick=EditorApplication.timeSinceStartup+2;
            Application.logMessageReceived-=Log; Application.logMessageReceived+=Log; EditorApplication.update+=Tick;
        }
        if(state==PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool("RoomCheckActive",false);
            if(SessionState.GetBool("RoomCheckPassed",false))EditorApplication.delayCall+=MP1cBuildTools.WireAndTest;
            else EditorApplication.Exit(1);
        }
    }
    static void Log(string message,string stack,LogType type)
    {
        if(type==LogType.Exception || (type==LogType.Error && !message.Contains("OpenXR") && !message.Contains("XR_ERROR")))runtimeFailure=new Exception(message+"\n"+stack);
    }
    static void Tick()
    {
        if(EditorApplication.timeSinceStartup<nextTick)return;
        try
        {
            if(runtimeFailure!=null)throw runtimeFailure;
            if(step==-1)
            {
                Check(SceneManager.GetActiveScene().name=="Start","Menu scene starts successfully");
                Check(!One<NewsPaperReader>().gameObject.activeInHierarchy,"Menu does not run a newspaper reader with no newspaper");
                var es=One<UnityEngine.EventSystems.EventSystem>();
                Check(es.GetComponents<UnityEngine.EventSystems.BaseInputModule>().Length==1&&es.GetComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(),"Menu uses one XR UI input module");
                One<StartSceneController>().WakeUp();
                step=0;nextTick=EditorApplication.timeSinceStartup+3;
                return;
            }
            if(step==0)
            {
                Check(SceneManager.GetActiveScene().name=="MP1B_EscapeRoom","WAKE UP loads the canonical gameplay scene");
                foreach(var go in SceneManager.GetActiveScene().GetRootGameObjects())if(go.name=="XR Interaction Simulator")go.SetActive(false);
                TestCollisions();
                supply=One<SupplyBoxPuzzle>(); cabinet=One<WallCabinetDoor>(); valves=One<ValveSequenceController>(); terminal=One<TerminalCodePuzzle>();
                key=(GameObject)Field(supply,"rewardItem");
                Check(!key.activeSelf,"Brass key is unavailable before packing");
                Check(cabinet.lockedCore&&!cabinet.lockedCore.enabled,"Generator core is locked before terminal success");
                packed=supply.transform.root.GetComponentsInChildren<PuzzleItem>();
                Check(packed.Length==4,"Storage has exactly four puzzle supplies; exit duplicates are separate");
                for(int i=0;i<3;i++)supply.TryPlaceItem(packed[i]);
                Check(!key.activeSelf,"Three storage supplies do not reveal the reward");
                supply.TryPlaceItem(packed[0]); Check(!key.activeSelf,"Repeated supply does not count twice");
                supply.TryPlaceItem(packed[3]); Check(key.activeSelf,"Four supply types reveal the brass key");
                Check(packed.All(i=>!i.GetComponent<XRGrabInteractable>().enabled&&i.GetComponent<Rigidbody>().isKinematic),"Stored supplies are secured after XR release");
                var locker=One<KeypadLockerController>(); var filter=(XRGrabInteractable)Field(locker,"lockedFilter");
                Check(!filter.enabled,"Locker filter cannot be grabbed before the code");
                locker.PressDigit(0); locker.Submit(); Check(!filter.enabled,"Wrong locker code leaves filter locked");
                foreach(char c in "2019")locker.PressDigit(c-'0'); locker.Submit(); Check(filter.enabled,"Correct locker code releases its filter");
                var leaks=One<WaterLeakPuzzle>();
                Check(valves.valveOrder.All(v=>!v.GetComponent<XRSimpleInteractable>().enabled),"Valves disabled before sealing");
                leaks.SealLeakA(); Check(valves.valveOrder.All(v=>!v.GetComponent<XRSimpleInteractable>().enabled),"One repaired leak is insufficient");
                leaks.SealLeakB(); Check(valves.valveOrder.All(v=>v.GetComponent<XRSimpleInteractable>().enabled),"Both leak events enable valves");
                valves.ValveTurned(valves.valveOrder.Last());
                step=1; nextTick=EditorApplication.timeSinceStartup+1.2;
            }
            else if(step==1)
            {
                Check((int)Field(valves,"currentStep")==0,"Wrong valve order resets progress");
                foreach(var v in valves.valveOrder)valves.ValveTurned(v);
                var gate=One<FuseBoxDoorGate>(); Check(gate.doorInteractable.enabled,"Valve sequence unlocks the fuse door");
                Check(!One<WaterLeakPuzzle>().leakA.isEmitting&&!One<WaterLeakPuzzle>().leakB.isEmitting,"Valve completion stops both leaks");
                gate.doorInteractable.selectEntered.Invoke(new SelectEnterEventArgs());
                Check(gate.interiorInteractables.All(i=>i.enabled),"Opening fuse door enables its controls");
                var fuse=One<FuseBoxPuzzle>();
                foreach(var t in fuse.targets)t.breaker.SetState(t.shouldBeOn);
                fuse.targets[0].breaker.SetState(!fuse.targets[0].shouldBeOn); fuse.CheckSolution();
                Check(!One<RadioDialController>().IsPowered,"Wrong breaker configuration leaves radio unpowered");
                fuse.targets[0].breaker.SetState(fuse.targets[0].shouldBeOn); fuse.CheckSolution();
                Check(One<RadioDialController>().IsPowered&&terminal.gameObject.activeInHierarchy,"Correct breakers power radio and terminal");
                step=2; nextTick=EditorApplication.timeSinceStartup+.5;
            }
            else if(step==2)
            {
                terminal.AddCharacter("A"); terminal.SubmitCode(); Check(!cabinet.lockedCore.enabled,"Wrong terminal code leaves core locked");
                terminal.ClearInput(); foreach(char c in "CS417")terminal.AddCharacter(c.ToString()); terminal.SubmitCode();
                Check(cabinet.lockedCore.enabled,"Correct terminal code enables core pickup");
                var coreSocket=One<GeneratorCoreSocket>(); Call(coreSocket,"OnTriggerStay",cabinet.lockedCore.GetComponent<Collider>());
                Check((bool)Field(coreSocket,"completed")&&cabinet.lockedCore.GetComponent<Rigidbody>().isKinematic,"Generator accepts and secures released core");
                var decon=One<DecontaminationPuzzleController>();
                var p=(XRSocketInteractor)Field(decon,"SocketParticulate"); var cSocket=(XRSocketInteractor)Field(decon,"SocketChemical"); var r=(XRSocketInteractor)Field(decon,"SocketRadiation");
                Call(decon,"Solve",new SelectEnterEventArgs{interactorObject=r}); Check((int)Field(decon,"currentStep")==0,"Wrong bedroom filter order resets progress");
                foreach(var socket in new[]{p,cSocket,r})Call(decon,"Solve",new SelectEnterEventArgs{interactorObject=socket});
                Check((bool)Field(decon,"solved")&&GameObject.Find("Keycard"),"Correct bedroom filter order spawns the keycard");
                step=3; nextTick=EditorApplication.timeSinceStartup+8;
            }
            else if(step==3)
            {
                Check(terminal.displayText.text.Contains("ACCESS GRANTED"),"Old rejection timer cannot erase terminal success");
                var capsule=One<GeneratorRewardCompartment>().authorizationCapsule;
                Check(capsule.activeSelf,"Core insertion reveals the power-cell clearance");
                var rewards=new[]{GameObject.Find("Keycard").GetComponent<XRGrabInteractable>(),capsule.GetComponent<XRGrabInteractable>(),key.GetComponent<XRGrabInteractable>()};
                var doorSockets=UnityEngine.Object.FindObjectsByType<FinalDoorSocket>().Select(s=>s.GetComponent<XRSocketInteractor>()).ToArray();
                foreach(var reward in rewards)
                    Check(doorSockets.Count(s=>(s.interactionLayers.value&reward.interactionLayers.value)!=0)==1,reward.name+" matches exactly one authorization socket");
                var manager=One<XRInteractionManager>();
                foreach(var reward in rewards)
                {
                    var socket=doorSockets.Single(s=>(s.interactionLayers.value&reward.interactionLayers.value)!=0);
                    manager.SelectEnter(socket,(IXRSelectInteractable)reward);
                }
                Check(Field(One<DepartureStationController>(),"currentStage").ToString()=="Packing","All three actual room rewards authorize departure packing");
                Check((bool)Field(One<FinalDoorController>(),"isUnlocked"),"Room rewards open inner authorization door");
                Check(!One<GameFlowController>().HasEnded,"Authorization does not stop the countdown");
                Check(!((GameObject)Field(One<FinalDoorController>(),"winPanel")).activeSelf,"Authorization does not show victory");
                var lockedHatch = One<BunkerHatchController>();
                Call(lockedHatch,"OnPush",new SelectEnterEventArgs());
                Check(!lockedHatch.IsReleased && (int)Field(lockedHatch,"pushes")==0,"Hatch rejects pushes before preparation");
                Call(One<WastelandEnding>(),"OnTriggerEnter",UnityEngine.Object.FindAnyObjectByType<CharacterController>());
                Check(!(bool)Field(One<WastelandEnding>(),"triggered"),"Outside trigger rejects bypassing preparation");
                var station=One<DepartureStationController>(); var sockets=(XRSocketInteractor[])Field(station,"supplySockets"); var items=(XRGrabInteractable[])Field(station,"supplyItems");
                foreach(var item in items)Check(sockets.Count(s=>(s.interactionLayers.value&item.interactionLayers.value)!=0)==1,item.name+" matches one exit slot");
                foreach(var item in packed)Check(sockets.All(s=>(s.interactionLayers.value&item.GetComponent<XRGrabInteractable>().interactionLayers.value)==0),item.name+" cannot substitute for EXIT supplies");
                for(int i=0;i<items.Length;i++)manager.SelectEnter(sockets[i],(IXRSelectInteractable)items[i]);
                station.SelectFilter(0);station.SelectFilter(1);station.SelectFilter(2);
                var breakers=(BreakerSwitch[])Field(station,"breakers");breakers[0].SetState(true);breakers[1].SetState(true);breakers[2].SetState(false);station.CheckPower();
                Check(One<BunkerHatchController>().IsReleased,"Full room-to-station sequence releases hatch");
                Check(!One<GameFlowController>().HasEnded,"Completing preparation keeps the countdown running");
                Call(One<WastelandEnding>(),"OnTriggerEnter",UnityEngine.Object.FindAnyObjectByType<CharacterController>());
                Check(!(bool)Field(One<WastelandEnding>(),"triggered"),"Outside trigger rejects a closed hatch");
                step=4;nextTick=EditorApplication.timeSinceStartup+2;
            }
            else if(step==4)
            {
                var hatch=One<BunkerHatchController>();
                for(int i=0;i<hatch.pushesRequired-1;i++)hatch.interactable.selectEntered.Invoke(new SelectEnterEventArgs());
                Check(!(bool)Field(hatch,"fullyOpened"),"Hatch requires all pushes");
                hatch.interactable.selectEntered.Invoke(new SelectEnterEventArgs());
                Check((bool)Field(hatch,"fullyOpened"),"Final hatch push opens the hatch");
                Call(One<WastelandEnding>(),"OnTriggerEnter",UnityEngine.Object.FindAnyObjectByType<CharacterController>());
                Check(!(bool)Field(One<WastelandEnding>(),"triggered"),"Ending waits for hatch to physically open");
                step=5;nextTick=EditorApplication.timeSinceStartup+3;
            }
            else if(step==5)
            {
                Check(One<BunkerHatchController>().IsOpen,"Hatch reaches its open angle");
                Call(One<WastelandEnding>(),"OnTriggerStay",UnityEngine.Object.FindAnyObjectByType<CharacterController>());
                Check(One<GameFlowController>().HasEnded,"Reaching outside after preparation and hatch opening stops timer");
                step=6;nextTick=EditorApplication.timeSinceStartup+4;
            }
            else if(step==6)
            {
                Check(One<WastelandEnding>().endingTextObject.activeSelf,"Exit trigger completes the ending presentation");
                One<GameFlowController>().RestartGame();
                step=7;nextTick=EditorApplication.timeSinceStartup+3;
            }
            else
            {
                Check(Field(One<DepartureStationController>(),"currentStage").ToString()=="AwaitingAuthorization","Restart resets the station to authorization");
                Check(!((GameObject)Field(One<SupplyBoxPuzzle>(),"rewardItem")).activeSelf,"Restart locks the storage reward again");
                Check(!(bool)Field(One<GameFlowController>(),"gameEnded")&&Time.timeScale==1,"Restart resumes the countdown");
                Check(!One<BunkerHatchController>().IsReleased && !One<BunkerHatchController>().IsOpen,"Restart relocks hatch");
                Check(!(bool)Field(One<WastelandEnding>(),"triggered"),"Restart clears ending state");
                Finish(true);
            }
        }
        catch(Exception e) { report.AppendLine("FAIL "+e); Debug.LogException(e); Finish(false); }
    }
    static void Finish(bool passed)
    {
        Application.logMessageReceived-=Log; EditorApplication.update-=Tick;
        File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"room-integration-check.txt"),report.ToString());
        SessionState.SetBool("RoomCheckPassed",passed); EditorApplication.isPlaying=false;
    }
}
