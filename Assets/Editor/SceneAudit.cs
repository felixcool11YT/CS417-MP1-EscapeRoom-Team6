using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public static class SceneAudit
{
    public static string PathOf(Transform t) => t.parent ? PathOf(t.parent) + "/" + t.name : t.name;
    public static void Run()
    {
        try
        {
            var report = new StringBuilder();
            foreach (var path in new[] { "Assets/EscapeRoom/Scenes/Start.unity", MP1cBuildTools.ScenePath })
            {
                var scene = EditorSceneManager.OpenScene(path);
                report.AppendLine("SCENE " + path);
                foreach (var root in scene.GetRootGameObjects())
                foreach (var t in root.GetComponentsInChildren<Transform>(true))
                {
                    int missing = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
                    if (missing > 0) report.AppendLine("MISSING SCRIPT " + PathOf(t));
                    foreach (var behaviour in t.GetComponents<MonoBehaviour>())
                    {
                        if (!behaviour) continue;
                        var script = MonoScript.FromMonoBehaviour(behaviour);
                        if (!AssetDatabase.GetAssetPath(script).StartsWith("Assets/EscapeRoom")) continue;
                        report.AppendLine("SCRIPT " + behaviour.GetType().Name + " " + PathOf(t) + " pos=" + t.position.ToString("F3") + " active=" + t.gameObject.activeInHierarchy);
                        var so = new SerializedObject(behaviour);
                        var p = so.GetIterator();
                        while (p.NextVisible(true))
                        {
                            if (p.propertyType == SerializedPropertyType.ObjectReference)
                                report.AppendLine("  " + p.propertyPath + " = " + (p.objectReferenceValue ? p.objectReferenceValue.name : "NULL"));
                            else if (p.propertyType == SerializedPropertyType.String || p.propertyType == SerializedPropertyType.Integer || p.propertyType == SerializedPropertyType.Boolean)
                                report.AppendLine("  " + p.propertyPath + " = " + (p.propertyType == SerializedPropertyType.String ? p.stringValue : p.propertyType == SerializedPropertyType.Boolean ? p.boolValue.ToString() : p.intValue.ToString()));
                        }
                    }
                    var text = t.GetComponent<TMP_Text>();
                    if (text) report.AppendLine("TEXT " + PathOf(t) + " pos=" + t.position.ToString("F3") + " forward=" + t.forward + " : " + text.text.Replace("\n", " | "));
                    var audio = t.GetComponent<AudioSource>();
                    if (audio) report.AppendLine("AUDIO " + PathOf(t) + " clip=" + AssetDatabase.GetAssetPath(audio.clip) + " length=" + (audio.clip ? audio.clip.length : 0) + " volume=" + audio.volume + " spatial=" + audio.spatialBlend + " range=" + audio.maxDistance + " loop=" + audio.loop + " playOnAwake=" + audio.playOnAwake);
                    var body = t.GetComponent<Rigidbody>();
                    if (body) report.AppendLine("BODY " + PathOf(t) + " mass=" + body.mass + " kinematic=" + body.isKinematic + " pos=" + t.position.ToString("F3"));
                    var canvas = t.GetComponent<Canvas>();
                    if (canvas) report.AppendLine("CANVAS " + PathOf(t) + " mode=" + canvas.renderMode + " distance=" + canvas.planeDistance);
                }
            }
            Directory.CreateDirectory(MP1cBuildTools.Output);
            File.WriteAllText(System.IO.Path.Combine(MP1cBuildTools.Output, "scene-audit.txt"), report.ToString());
            EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogException(e); EditorApplication.Exit(1); }
    }
}
