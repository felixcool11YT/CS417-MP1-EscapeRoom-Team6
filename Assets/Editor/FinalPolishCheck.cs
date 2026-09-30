using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[InitializeOnLoad]
public static class FinalPolishCheck
{
    static readonly BindingFlags Flags=BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic;
    static readonly StringBuilder report=new StringBuilder();
    static int step; static double nextTick; static Exception failure;
    static NewsPaperReader reader; static Camera camera; static Canvas canvas;
    static GameObject note,panel,prompt; static GeneratorRewardCompartment reward;
    static Keyboard keyboard;
    static InputSettings originalInputSettings,testInputSettings;
    static object Field(object target,string name)=>target.GetType().GetField(name,Flags).GetValue(target);
    static void Call(object target,string method,params object[] args)=>target.GetType().GetMethod(method,Flags).Invoke(target,args);
    static T One<T>() where T:UnityEngine.Object=>UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include).Single();
    static void Check(bool ok,string message){if(!ok)throw new Exception(message);report.AppendLine("PASS "+message);}
    static FinalPolishCheck(){EditorApplication.playModeStateChanged+=Changed;}
    public static void Run()
    {
        EditorSceneManager.OpenScene(MP1cBuildTools.ScenePath);
        SessionState.SetBool("FinalPolishActive",true);SessionState.SetBool("FinalPolishPassed",false);
        EditorApplication.isPlaying=true;
    }
    static void Changed(PlayModeStateChange state)
    {
        if(!SessionState.GetBool("FinalPolishActive",false))return;
        if(state==PlayModeStateChange.EnteredPlayMode)
        {
            report.Clear();step=0;failure=null;nextTick=EditorApplication.timeSinceStartup+2;
            EditorApplication.update+=Tick;Application.logMessageReceived+=Log;
        }
        if(state==PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool("FinalPolishActive",false);
            if(SessionState.GetBool("FinalPolishPassed",false))EditorApplication.delayCall+=GrabSafetyCheck.Run;
            else EditorApplication.Exit(1);
        }
    }
    static void Log(string message,string stack,LogType type)
    {
        if(type==LogType.Exception||(type==LogType.Error&&!message.Contains("OpenXR")&&!message.Contains("XR_ERROR")))failure=new Exception(message+"\n"+stack);
    }
    static void Aim(GameObject target,float distance)
    {
        var center=target.GetComponentInChildren<Collider>().bounds.center;
        camera.transform.SetPositionAndRotation(center-Vector3.forward*distance,Quaternion.identity);
        Physics.SyncTransforms();Call(reader,"Update");
    }
    static void Key(bool down)
    {
        InputSystem.QueueStateEvent(keyboard,down?new KeyboardState(UnityEngine.InputSystem.Key.V):new KeyboardState());
        InputSystem.Update();
        if(down)Check(keyboard.vKey.isPressed,"Synthetic read key reaches the input system");
    }
    static void Capture()
    {
        var original=QualitySettings.renderPipeline;
        var copy=UnityEngine.Object.Instantiate(GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset);
        copy.useSRPBatcher=false;copy.gpuResidentDrawerMode=GPUResidentDrawerMode.Disabled;QualitySettings.renderPipeline=copy;
        var rt=new RenderTexture(1920,1080,24);camera.targetTexture=rt;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=rt;
        var image=new Texture2D(1920,1080,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1920,1080),0,0);image.Apply();
        File.WriteAllBytes(Path.Combine(MP1cBuildTools.Output,"departure-note-camera-check.png"),image.EncodeToPNG());
        RenderTexture.active=null;camera.targetTexture=null;UnityEngine.Object.DestroyImmediate(image);UnityEngine.Object.DestroyImmediate(rt);
        QualitySettings.renderPipeline=original;UnityEngine.Object.DestroyImmediate(copy);
    }
    static void Tick()
    {
        if(EditorApplication.timeSinceStartup<nextTick)return;
        try
        {
            if(failure!=null)throw failure;
            switch(step)
            {
                case 0:
                    var simulator=GameObject.Find("XR Interaction Simulator");if(simulator)simulator.SetActive(false);
                    reader=One<NewsPaperReader>();camera=(Camera)Field(reader,"playerCamera");canvas=reader.GetComponent<Canvas>();
                    foreach(var b in camera.GetComponents<Behaviour>())if(b.GetType().Name.Contains("TrackedPoseDriver"))b.enabled=false;
                    note=GameObject.Find("MP1C_DepartureStation/04_R_Instructions/R_DepartureNote");
                    panel=reader.transform.Find("DepartureFullPanel").gameObject;prompt=reader.transform.Find("DepartureReadPrompt").gameObject;
                    Check(!panel.activeSelf&&!prompt.activeSelf,"Departure reading UI starts hidden");
                    Check(!GameObject.Find("MP1C_DepartureStation").transform.Find("07_NoteReadingPanel"),"Fixed world reading panel has been removed");
                    Check(canvas.renderMode==RenderMode.ScreenSpaceCamera&&canvas.worldCamera==camera&&reader.transform.IsChildOf(camera.transform),"Both papers share the newspaper camera canvas");
                    var action=((InputActionReference)Field(reader,"readAction")).action;
                    Check(action.bindings.Any(b=>b.path=="<XRController>{LeftHand}/primaryButton")&&action.bindings.Any(b=>b.path=="<Keyboard>/v"),"Original headset X and desktop V bindings are preserved");
                    originalInputSettings=InputSystem.settings;testInputSettings=UnityEngine.Object.Instantiate(originalInputSettings);
                    testInputSettings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
                    testInputSettings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
                    InputSystem.settings=testInputSettings;
                    keyboard=InputSystem.AddDevice<Keyboard>("ReadingTestKeyboard");InputSystem.EnableDevice(keyboard);
                    Check(action.enabled,"Shared read action is enabled");
                    Aim(note,1);Check(prompt.activeSelf,"Looking at R's note shows the read prompt");Key(true);
                    reward=One<GeneratorRewardCompartment>();reward.RevealReward();
                    break;
                case 1:
                    Check(panel.activeSelf&&!prompt.activeSelf,"Read input opens the departure note and hides its prompt");Key(false);
                    camera.transform.SetPositionAndRotation(camera.transform.position+new Vector3(3,1,2),Quaternion.Euler(12,110,0));Canvas.ForceUpdateCanvases();
                    var center=camera.WorldToViewportPoint(panel.transform.position);
                    Check(Mathf.Abs(center.x-.5f)<.01f&&Mathf.Abs(center.y-.5f)<.01f&&Mathf.Abs(center.z-.15f)<.01f,"Open note stays centered at the newspaper depth after camera movement");
                    var clue=panel.transform.Find("Instructions").GetComponent<TMP_Text>();clue.ForceMeshUpdate();
                    Check(!clue.isTextOverflowing&&clue.text.Length>100,"Full departure clue fits in its paper panel");Capture();
                    break;
                case 2: Key(true);break;
                case 3:
                    Check(!panel.activeSelf,"Read input closes the departure note");Key(false);
                    Aim(note,5);Check(!prompt.activeSelf,"Distant departure note does not show a read prompt");
                    Aim(note,1);note.GetComponent<XRSimpleInteractable>().selectEntered.Invoke(new SelectEnterEventArgs());
                    Check(panel.activeSelf,"Ray selection opens the same departure camera panel");
                    break;
                case 4: Key(true);break;
                case 5:
                    Check(!panel.activeSelf,"Read input also closes a ray-opened note");Key(false);
                    Aim((GameObject)Field(reader,"newsPaper"),1);
                    Check(((GameObject)Field(reader,"newsPaperPanel")).activeSelf&&!prompt.activeSelf,"Bedroom newspaper still has its own gaze prompt");
                    break;
                case 6: Key(true);break;
                case 7:
                    Check(((GameObject)Field(reader,"newspaperFullPanel")).activeSelf&&!panel.activeSelf,"Original newspaper still opens with the same read input");Key(false);
                    var progress=One<PuzzleProgressDisplay>();var sound=(AudioSource)Field(progress,"placementFeedbackAudio");
                    Check(sound.clip&&sound.clip.length<1&&!sound.loop&&!sound.playOnAwake&&sound.spatialBlend==1,"Filter placement uses a short local non-looping sound");
                    sound.Stop();progress.ShowProgress(0);Check(!sound.isPlaying,"Filter initialization stays silent");
                    progress.ShowProgress(1);Check(sound.isPlaying,"First correct filter plays placement feedback");
                    sound.Stop();progress.ShowProgress(2);Check(sound.isPlaying,"Second correct filter plays placement feedback");
                    progress.ShowComplete();Check(((AudioSource)Field(progress,"positiveFeedbackAudio")).isPlaying,"Final filter retains puzzle completion feedback");
                    Check(reward.authorizationCapsule.activeSelf&&Vector3.Distance(reward.hatchDoor.localPosition,reward.openPosition)<.002f,"Reward compartment stays open beyond the former closing delay");
                    Finish(true);return;
            }
            step++;nextTick=EditorApplication.timeSinceStartup+.7;
        }
        catch(Exception e){report.AppendLine("FAIL "+e);Debug.LogException(e);Finish(false);}
    }
    static void Finish(bool passed)
    {
        if(keyboard!=null){Key(false);InputSystem.RemoveDevice(keyboard);}
        if(originalInputSettings!=null)InputSystem.settings=originalInputSettings;
        if(testInputSettings!=null)UnityEngine.Object.DestroyImmediate(testInputSettings);
        EditorApplication.update-=Tick;Application.logMessageReceived-=Log;
        File.WriteAllText(Path.Combine(MP1cBuildTools.Output,"final-polish-check.txt"),report.ToString());
        SessionState.SetBool("FinalPolishPassed",passed);EditorApplication.isPlaying=false;
    }
}
