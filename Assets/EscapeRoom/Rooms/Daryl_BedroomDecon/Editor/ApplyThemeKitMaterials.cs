using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Applies the theme kit materials to all models in the Models folder.
/// Run via Tools > Theme Kit > Apply Materials to Models.
/// Sets Material Search to ProjectWide so Unity auto-matches material slots by name
/// (e.g. "MAT_Wood") to .mat files in the project.
/// </summary>
public static class ApplyThemeKitMaterials
{
    [MenuItem("Tools/Theme Kit/Apply Materials to Models")]
    public static void ApplyMaterials()
    {
        string modelsDir = "Assets/EscapeRoom/Rooms/Daryl_BedroomDecon/Models";

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

            // Set material search to find materials by name across the project
            // This matches usemtl names (e.g. "MAT_Wood") to .mat files
            if (importer.materialSearch != ModelImporterMaterialSearch.ProjectWide)
            {
                importer.materialSearch = ModelImporterMaterialSearch.ProjectWide;
                importer.SaveAndReimport();
                fixedCount++;
                Debug.Log($"[ThemeKit] Enabled ProjectWide material search for {Path.GetFileName(objPath)}");
            }
        }

        Debug.Log($"[ThemeKit] Done. Updated {fixedCount} of {objFiles.Length} models. Materials should now auto-apply by name.");
    }
}
