using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Applies the theme kit materials to all models in the Models folder.
/// Run via Tools > Theme Kit > Apply Materials to Models.
/// Matches material slots by name (e.g. "MAT_Wood") to .mat files in the Materials folder.
/// </summary>
public static class ApplyThemeKitMaterials
{
    [MenuItem("Tools/Theme Kit/Apply Materials to Models")]
    public static void ApplyMaterials()
    {
        string modelsDir = "Assets/EscapeRoom/Rooms/Daryl_BedroomDecon/Models";
        string matsDir = "Assets/EscapeRoom/Rooms/Daryl_BedroomDecon/Materials";

        if (!Directory.Exists(modelsDir))
        {
            Debug.LogError("[ThemeKit] Models folder not found: " + modelsDir);
            return;
        }

        string[] objFiles = Directory.GetFiles(modelsDir, "*.obj");
        int fixedCount = 0;

        foreach (string objPath in objFiles)
        {
            var importer = AssetImporter.GetAtPath(objPath) as ModelImporter;
            if (importer == null) continue;

            var map = importer.GetExternalObjectMap();
            var newMap = new Dictionary<SourceAssetIdentifier, Object>(map);
            bool changed = false;

            foreach (var kvp in map)
            {
                if (kvp.Key.type != typeof(Material)) continue;

                string matName = kvp.Key.name;
                string matPath = (matsDir + "/" + matName + ".mat").Replace("\\", "/");
                var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

                if (mat != null && kvp.Value != mat)
                {
                    newMap[kvp.Key] = mat;
                    changed = true;
                    Debug.Log($"[ThemeKit] {Path.GetFileName(objPath)}: assigned {matName}");
                }
                else if (mat == null)
                {
                    Debug.LogWarning($"[ThemeKit] {Path.GetFileName(objPath)}: material not found: {matName}");
                }
            }

            if (changed)
            {
                importer.SetExternalObjectMap(newMap);
                importer.SaveAndReimport();
                fixedCount++;
            }
        }

        Debug.Log($"[ThemeKit] Done. Updated {fixedCount} of {objFiles.Length} models.");
    }
}
