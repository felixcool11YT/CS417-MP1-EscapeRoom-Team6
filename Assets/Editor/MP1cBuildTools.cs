using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using TMPro;

public static class MP1cBuildTools
{
    public const string ScenePath="Assets/EscapeRoom/Scenes/MP1B_EscapeRoom.unity";
    public static string Output=>Path.GetFullPath("../the_room_info/MP1c_Quest_Test");
    static void Reference(SerializedObject so,string field,UnityEngine.Object value){if(!value)throw new Exception("Missing "+field);so.FindProperty(field).objectReferenceValue=value;}
    static void References(SerializedObject so,string field,UnityEngine.Object[] values){if(values.Any(v=>!v))throw new Exception("Missing "+field);var p=so.FindProperty(field);p.arraySize=values.Length;for(int i=0;i<values.Length;i++)p.GetArrayElementAtIndex(i).objectReferenceValue=values[i];}
    public static void WireAndTest()
    {
        try
        {
            Directory.CreateDirectory(Output);
            var scene=EditorSceneManager.OpenScene(ScenePath);
            var roots=scene.GetRootGameObjects();var root=roots.Single(g=>g.name=="MP1C_DepartureStation").transform;
            var controller=root.GetComponent<DepartureStationController>()??root.gameObject.AddComponent<DepartureStationController>();
            var filterMethod=typeof(DepartureStationController).GetMethod("SelectFilter");if(filterMethod==null)throw new Exception("Departure station is missing SelectFilter.");
            var pack=root.Find("01_PackDepartureKit");var purge=root.Find("02_PurgeExitAir");var power=root.Find("03_PowerOuterDoor");
            var so=new SerializedObject(controller);
            References(so,"supplySockets",new UnityEngine.Object[]{pack.Find("SupplySlots/FirstAidSlot").GetComponent<XRSocketInteractor>(),pack.Find("SupplySlots/RadioSlot").GetComponent<XRSocketInteractor>()});
            References(so,"supplyItems",new UnityEngine.Object[]{pack.Find("EXIT_FirstAidKit").GetComponent<XRGrabInteractable>(),pack.Find("EXIT_Radio").GetComponent<XRGrabInteractable>()});
            Reference(so,"packingText",pack.Find("PackingStatus").GetComponent<TMP_Text>());
            string[] filters={"Particulate","Chemical","Radiation"};var controls=filters.Select(n=>purge.Find("FilterControl_"+n).GetComponent<XRSimpleInteractable>()).ToArray();References(so,"filterControls",controls);
            for(int i=0;i<controls.Length;i++)
            {
                for(int j=controls[i].selectEntered.GetPersistentEventCount()-1;j>=0;j--)if(controls[i].selectEntered.GetPersistentTarget(j)==controller)UnityEditor.Events.UnityEventTools.RemovePersistentListener(controls[i].selectEntered,j);
                var action=(UnityAction<int>)Delegate.CreateDelegate(typeof(UnityAction<int>),controller,filterMethod);
                UnityEditor.Events.UnityEventTools.AddIntPersistentListener(controls[i].selectEntered,action,i);
            }
            References(so,"filterLamps",Enumerable.Range(1,3).Select(i=>(UnityEngine.Object)purge.Find("FilterProgressLamps/Step"+i).GetComponent<Renderer>()).ToArray());
            Reference(so,"purgeText",purge.Find("StationStatus").GetComponent<TMP_Text>());
            References(so,"breakers",new[]{"FILTERS","DOOR","AUX"}.Select(n=>(UnityEngine.Object)power.Find("Breaker_"+n).GetComponent<BreakerSwitch>()).ToArray());
            var test=power.Find("TestButton").GetComponent<XRSimpleInteractable>();Reference(so,"testButton",test);
            for(int j=test.selectEntered.GetPersistentEventCount()-1;j>=0;j--)if(test.selectEntered.GetPersistentTarget(j)==controller)UnityEditor.Events.UnityEventTools.RemovePersistentListener(test.selectEntered,j);
            UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(test.selectEntered,controller.CheckPower);
            Reference(so,"stationText",root.Find("00_StationFrame/StationStatus").GetComponent<TMP_Text>());
            References(so,"stageLamps",new UnityEngine.Object[]{pack.Find("StatusLamp").GetComponent<Renderer>(),purge.Find("StatusLamp").GetComponent<Renderer>(),power.Find("StatusLamp").GetComponent<Renderer>()});
            string mat="Assets/EscapeRoom/Shared/FinalChallenge/Materials/";
            Reference(so,"neutralMaterial",AssetDatabase.LoadAssetAtPath<Material>(mat+"MAT_StationInset.mat"));Reference(so,"readyMaterial",AssetDatabase.LoadAssetAtPath<Material>(mat+"MAT_StatusReady.mat"));Reference(so,"completeMaterial",AssetDatabase.LoadAssetAtPath<Material>(mat+"MAT_StatusComplete.mat"));
            Reference(so,"acceptedAudio",root.Find("06_StationFeedback/AcceptedAudio").GetComponent<AudioSource>());Reference(so,"rejectedAudio",root.Find("06_StationFeedback/RejectedAudio").GetComponent<AudioSource>());Reference(so,"completedAudio",root.Find("06_StationFeedback/CompletedAudio").GetComponent<AudioSource>());
            var door=roots.SelectMany(g=>g.GetComponentsInChildren<FinalDoorController>(true)).Single();Reference(so,"finalDoor",door);so.ApplyModifiedPropertiesWithoutUndo();
            var doorSo=new SerializedObject(door);Reference(doorSo,"departureStation",controller);doorSo.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();
            DepartureStationIntegrationCheck.Begin();
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
    public static void BuildQuest()
    {
        try
        {
            // Always load the SAVED scenes after the unsaved isolated test.
            EditorSceneManager.OpenScene(ScenePath);Directory.CreateDirectory(Output);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android,ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel32;
            PlayerSettings.Android.bundleVersionCode=2;
            EditorUserBuildSettings.buildAppBundle=false;
            string apk=Path.Combine(Output,"Team6_MP1c_Quest_Test.apk");
            var result=BuildPipeline.BuildPlayer(new BuildPlayerOptions{scenes=new[]{"Assets/EscapeRoom/Scenes/Start.unity",ScenePath},locationPathName=apk,target=BuildTarget.Android,options=BuildOptions.None});
            File.WriteAllText(Path.Combine(Output,"build-report.txt"),"Result: "+result.summary.result+"\nErrors: "+result.summary.totalErrors+"\nSize: "+result.summary.totalSize+"\nDuration: "+result.summary.totalTime+"\nAPK: "+apk);
            if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Quest APK build failed");
            Debug.Log("MP1C_QUEST_BUILD_SUCCESS "+apk);EditorApplication.Exit(0);
        }catch(Exception e){Debug.LogException(e);EditorApplication.Exit(1);}
    }
}
// The desktop simulator must not override the headset's real tracked devices.
public class MP1cHeadsetSceneProcessor : UnityEditor.Build.IProcessSceneWithReport
{
    public int callbackOrder=>0;
    public void OnProcessScene(UnityEngine.SceneManagement.Scene scene,BuildReport report)
    {
        if(report==null || report.summary.platform!=BuildTarget.Android)return;
        foreach(var root in scene.GetRootGameObjects())if(root.name=="XR Interaction Simulator")UnityEngine.Object.DestroyImmediate(root);
    }
}
