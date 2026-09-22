// Repairs mesh links in the bedroom/decon theme kit prefabs.
//
// The prefabs were generated outside Unity and reference their model meshes
// with a placeholder fileID, so Unity shows them as "Missing". This tool
// resolves the real fileID for each mesh through the AssetDatabase and
// rewrites the prefab files. It runs automatically once after import, and
// can be re-run any time via Tools > Theme Kit > Repair Mesh Links.
//
// It only touches prefabs in this room's Prefabs folder. Nothing else in
// the project is modified.
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class ThemeKitMeshRepair
{
    const string PrefabFolder = "Assets/EscapeRoom/Rooms/Daryl_BedroomDecon/Prefabs";
    const string SessionKey = "ThemeKitMeshRepair.Done";
    const string BrokenPattern = @"m_Mesh: \{fileID: 4300000, guid: ([0-9a-f]{32}), type: 3\}";

    [InitializeOnLoadMethod]
    static void AutoRunOnce()
    {
        EditorApplication.delayCall += () =>
        {
            if (SessionState.GetBool(SessionKey, false))
                return;
            int fixedCount = RepairAll(quiet: true);
            if (fixedCount > 0)
                Debug.Log($"[ThemeKit] Auto-repaired mesh links in {fixedCount} prefab(s).");
            SessionState.SetBool(SessionKey, true);
        };
    }

    [MenuItem("Tools/Theme Kit/Repair Mesh Links")]
    static void RepairFromMenu()
    {
        int fixedCount = RepairAll(quiet: false);
        SessionState.SetBool(SessionKey, true);
        Debug.Log($"[ThemeKit] Mesh link repair finished. Fixed {fixedCount} prefab(s). " +
                  "If a scene instance still looks empty, delete it and drag the prefab in again.");
    }

    static int RepairAll(bool quiet)
    {
        int fixedCount = 0;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder });
        foreach (string prefabGuid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            string text = File.ReadAllText(path);
            bool changed = false;
            string updated = Regex.Replace(text, BrokenPattern, match =>
            {
                string meshGuid = match.Groups[1].Value;
                string modelPath = AssetDatabase.GUIDToAssetPath(meshGuid);
                if (string.IsNullOrEmpty(modelPath))
                {
                    if (!quiet) Debug.LogWarning($"[ThemeKit] No asset found for mesh GUID {meshGuid} in {path}");
                    return match.Value;
                }
                Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(modelPath);
                if (mesh == null)
                {
                    if (!quiet) Debug.LogWarning($"[ThemeKit] Could not load mesh from {modelPath}");
                    return match.Value;
                }
                if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(mesh, out _, out long fileId))
                {
                    if (!quiet) Debug.LogWarning($"[ThemeKit] Could not get fileID for mesh {mesh.name}");
                    return match.Value;
                }
                changed = true;
                return $"m_Mesh: {{fileID: {fileId}, guid: {meshGuid}, type: 3}}";
            });
            if (changed)
            {
                File.WriteAllText(path, updated);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                fixedCount++;
                if (!quiet) Debug.Log($"[ThemeKit] Repaired mesh links in {path}");
            }
        }
        return fixedCount;
    }
}
