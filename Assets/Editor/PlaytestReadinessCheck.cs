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
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Casters;

[InitializeOnLoad]
public static class PlaytestReadinessCheck
{
    const string Active = "PlaytestReadinessActive", Passed = "PlaytestReadinessPassed";
    static readonly BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
    static readonly StringBuilder report = new StringBuilder();
    static IEnumerator<float> checks;
    static double next;
    static Exception runtimeFailure;
    static object Field(object target, string name) => target.GetType().GetField(name, Flags).GetValue(target);
    static void Call(object target, string name, params object[] args) => target.GetType().GetMethod(name, Flags).Invoke(target, args);
    static void Check(bool ok, string description)
    {
        if (!ok) throw new Exception(description);
        report.AppendLine("PASS " + description);
    }
    static PlaytestReadinessCheck() { EditorApplication.playModeStateChanged += Changed; }
    public static void Run()
    {
        EditorSceneManager.OpenScene("Assets/EscapeRoom/Scenes/Start.unity");
        // Use the saved headset rig; only the test supplies its controller poses and inputs.
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == "XR Interaction Simulator") root.SetActive(false);
        SessionState.SetBool(Active, true);
        SessionState.SetBool(Passed, false);
        EditorApplication.isPlaying = true;
    }
    static void Changed(PlayModeStateChange state)
    {
        if (!SessionState.GetBool(Active, false)) return;
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            report.Clear(); runtimeFailure = null;
            checks = RunChecks(); next = EditorApplication.timeSinceStartup + 3;
            EditorApplication.update += Tick;
            Application.logMessageReceived += Log;
        }
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool(Active, false);
            if (SessionState.GetBool(Passed, false)) EditorApplication.delayCall += FinalPolishCheck.Run;
            else EditorApplication.Exit(1);
        }
    }
    static void Log(string message, string stack, LogType type)
    {
        if (type == LogType.Exception) runtimeFailure = new Exception(message + "\n" + stack);
    }
    static void Tick()
    {
        if (EditorApplication.timeSinceStartup < next) return;
        try
        {
            if (runtimeFailure != null) throw runtimeFailure;
            if (checks.MoveNext()) { next = EditorApplication.timeSinceStartup + checks.Current; return; }
            Finish(true);
        }
        catch (Exception e) { report.AppendLine("FAIL " + e); Debug.LogException(e); Finish(false); }
    }
    static void Finish(bool passed)
    {
        Directory.CreateDirectory(MP1cBuildTools.Output);
        File.WriteAllText(Path.Combine(MP1cBuildTools.Output, "playtest-readiness.txt"), report.ToString());
        SessionState.SetBool(Passed, passed);
        Application.logMessageReceived -= Log;
        EditorApplication.update -= Tick;
        EditorApplication.isPlaying = false;
    }
    static IEnumerator<float> RunChecks()
    {
        UnityEngine.Object.FindAnyObjectByType<StartSceneController>().WakeUp();
        yield return 2;
        var managers = UnityEngine.Object.FindObjectsByType<XRInteractionManager>();
        Check(managers.Length == 1, "Menu-to-game transition leaves one interaction manager");
        var station = UnityEngine.Object.FindAnyObjectByType<DepartureStationController>();
        var door = UnityEngine.Object.FindAnyObjectByType<FinalDoorController>();
        var items = (XRGrabInteractable[])Field(station, "supplyItems");
        var sockets = (XRSocketInteractor[])Field(station, "supplySockets");
        Check(items.All(i => !i.enabled), "Supplies locked before authorization");
        door.RegisterItem(); door.RegisterItem();
        Check(items.All(i => !i.enabled), "Two clearances do not unlock supplies");
        door.RegisterItem();
        yield return 1;
        Check(items.All(i => i.isActiveAndEnabled), "Third clearance unlocks both supplies in the full scene");
        var shelf = station.transform.Find("01_PackDepartureKit/SupplyShelf").GetComponent<Collider>();
        Check(shelf.enabled && !shelf.isTrigger && shelf.gameObject.layer == 2,
            "Shelf remains physically solid but does not block controller targeting");
        var hands = UnityEngine.Object.FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include);
        Check(hands.Length == 2, "Both saved controller interactors present");
        Check(items.All(i => i.interactionManager == managers[0]) && hands.All(h => h.interactionManager == managers[0]),
            "Controllers and departure supplies share the manager after Wake Up");
        foreach (var hand in hands)
        {
            hand.containingGroup?.RemoveGroupMember(hand);
            hand.transform.SetParent(null);
            hand.gameObject.SetActive(true); hand.enabled = true;
            var aim = new GameObject("ControllerTestPose").transform;
            hand.farInteractionCaster.castOrigin = aim;
            hand.nearInteractionCaster.castOrigin = aim;
            ((CurveInteractionCaster)hand.farInteractionCaster).enableStabilization = false;
            hand.selectInput.inputSourceMode = XRInputButtonReader.InputSourceMode.ManualValue;
            foreach (var item in items)
            {
                foreach (float height in new[] { 1f, 1.3f, 1.65f })
                {
                    var center = item.colliders[0].bounds.center;
                    var position = center + Vector3.left * .65f; position.y = height;
                    aim.SetPositionAndRotation(position, Quaternion.LookRotation(center - position));
                    hand.transform.SetPositionAndRotation(position, aim.rotation);
                    Physics.SyncTransforms();
                    yield return .25f;
                    var targets = new List<IXRInteractable>(); hand.GetValidTargets(targets);
                    Check(targets.Contains(item), hand.handedness + " ray finds " + item.name + " at height " + height);
                }
                var nearPosition = item.colliders[0].bounds.center + Vector3.left * .07f;
                aim.position = nearPosition; hand.transform.position = nearPosition;
                yield return .25f;
                var nearTargets = new List<IXRInteractable>(); hand.GetValidTargets(nearTargets);
                Check(nearTargets.Contains(item), hand.handedness + " near grab finds " + item.name);
                var originalPosition = item.transform.position; var originalRotation = item.transform.rotation;
                hand.selectInput.QueueManualState(true, 1);
                yield return .25f;
                Check(item.interactorsSelecting.Contains(hand), hand.handedness + " grip selects " + item.name);
                aim.position += Vector3.up * .3f + Vector3.left * .2f;
                hand.transform.position = aim.position;
                yield return .6f;
                Check(item.transform.position.y > originalPosition.y + .1f,
                    hand.handedness + " lifts " + item.name + " off shelf");
                hand.selectInput.QueueManualState(false, 0);
                yield return .2f;
                Check(!item.isSelected, hand.handedness + " release drops " + item.name);
                hand.transform.position = aim.position = new Vector3(0, 10, 0);
                var body = item.GetComponent<Rigidbody>();
                body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero;
                body.position = originalPosition; body.rotation = originalRotation;
                item.transform.SetPositionAndRotation(originalPosition, originalRotation);
                Physics.SyncTransforms();
                yield return .3f;
            }
            hand.gameObject.SetActive(false);
        }
        for (int i = 0; i < items.Length; i++)
        {
            Check(!sockets[1-i].interactionManager.IsSelectPossible((IXRSelectInteractor)sockets[1-i], (IXRSelectInteractable)items[i]), "Wrong supply slot rejects " + items[i].name);
            items[i].transform.position += sockets[i].attachTransform.position - items[i].attachTransform.position;
            var body = items[i].GetComponent<Rigidbody>(); body.position = items[i].transform.position;
            body.linearVelocity = Vector3.zero; body.angularVelocity = Vector3.zero;
            Physics.SyncTransforms();
            yield return 1;
        }
        Check(Field(station, "currentStage").ToString() == "Purging", "Physical packing advances full-scene station");
        Check(items.All(i => !i.enabled && i.GetComponent<Rigidbody>().isKinematic), "Packed supplies stay secured");
        var ending = UnityEngine.Object.FindAnyObjectByType<WastelandEnding>();
        var hatch = UnityEngine.Object.FindAnyObjectByType<BunkerHatchController>();
        var flow = UnityEngine.Object.FindAnyObjectByType<GameFlowController>();
        var camera = Camera.main;
        var volume = ending.GetComponent<BoxCollider>();
        var exit = volume.transform.TransformPoint(volume.center) + Vector3.up;
        camera.transform.position = exit; Call(ending, "Update");
        Check(!flow.HasEnded, "Exit cannot bypass unfinished station");
        station.SelectFilter(2);
        Check((int)Field(station, "filterStep") == 0, "Incorrect filter sequence is rejected");
        station.SelectFilter(0); station.SelectFilter(1); station.SelectFilter(2);
        Check(Field(station, "currentStage").ToString() == "Powering", "Correct purge unlocks breaker stage");
        var breakers = (BreakerSwitch[])Field(station, "breakers");
        breakers[0].SetState(true); breakers[1].SetState(true); breakers[2].SetState(false); station.CheckPower();
        Check(station.IsComplete && hatch.IsReleased, "Correct breakers release hatch");
        Call(ending, "Update"); Check(!flow.HasEnded, "Closed hatch still prevents escape");
        for(int i = 0; i < hatch.pushesRequired; i++) Call(hatch, "OnPush", new SelectEnterEventArgs());
        camera.transform.position = volume.transform.TransformPoint(volume.center + Vector3.right * (volume.size.x + 10));
        yield return 3;
        Check(hatch.IsOpen, "Hatch pushes finish opening animation");
        Call(ending, "Update"); Check(!flow.HasEnded, "Outside exit footprint does not complete game");
        camera.transform.position = exit; Call(ending, "Update");
        Check(flow.HasEnded, "Tracked head crossing exit completes game");
        yield return 4;
        Check(ending.endingTextObject.activeInHierarchy, "Escape message becomes visible");
        Check(ending.endingCanvas.parent == camera.transform, "Escape message follows headset");
        Check(ending.whiteFlash.alpha < .01f, "Ending flash clears");
    }
}
