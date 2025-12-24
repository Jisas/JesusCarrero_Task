using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FBXMaterialFixer : EditorWindow
{
    [MenuItem("Tools/FBX Material Remaper")]
    public static void ShowWindow() => GetWindow<FBXMaterialFixer>("FBX Material Remaper");

    void OnGUI()
    {
        EditorGUILayout.HelpBox("This method reads the internal FBX table even if the materials were deleted.", MessageType.Info);
        if (GUILayout.Button("Link by Internal Names", GUILayout.Height(40)))
        {
            FixFBXBruteForce();
        }
    }

    void FixFBXBruteForce()
    {
        Object selected = Selection.activeObject;
        string path = AssetDatabase.GetAssetPath(selected);
        ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

        if (importer == null)
        {
            Debug.LogError("Select the FBX in the Project window.");
            return;
        }

        // 1. Upload the actual project materials.
        Dictionary<string, Material> projectMaterials = new Dictionary<string, Material>();
        string[] allMatGuids = AssetDatabase.FindAssets("t:Material");

        foreach (string guid in allMatGuids)
        {
            string matPath = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

            if (mat != null && !projectMaterials.ContainsKey(mat.name))
                projectMaterials.Add(mat.name, mat);
        }

        // 2. Use SerializedObject to read the list of materials from the Importer.
        // Even if the FBX does not have “live” materials, the Importer saves the names.
        SerializedObject so = new SerializedObject(importer);
        SerializedProperty externalObjects = so.FindProperty("m_ExternalObjects");
        SerializedProperty materialsProp = so.FindProperty("m_Materials"); // Internal list of names

        int count = 0;

        // Try to get the names from the internal property m_Materials
        if (materialsProp != null && materialsProp.isArray)
        {
            for (int i = 0; i < materialsProp.arraySize; i++)
            {
                SerializedProperty matEntry = materialsProp.GetArrayElementAtIndex(i);
                string internalName = matEntry.FindPropertyRelative("name").stringValue;

                if (string.IsNullOrEmpty(internalName)) continue;

                // Clean name: “Window.001” -> “Window”
                string cleanName = Regex.Replace(internalName, @"(\.\d+)|(\s\(Instance\))$", "");

                Debug.Log($"<color=cyan>Analyzing slot:</color> {internalName} -> Looking for: {cleanName}");

                if (projectMaterials.TryGetValue(cleanName, out Material targetMat))
                {
                    // We create the identifier for remapping
                    var identifier = new AssetImporter.SourceAssetIdentifier(typeof(Material), internalName);
                    importer.AddRemap(identifier, targetMat);
                    count++;
                }
            }
        }

        if (count > 0)
        {
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            AssetDatabase.Refresh();
            Debug.Log($"<color=green><b>Got it!</b></color> They have been remapped {count} material slots.");
        }
        else
        {
            Debug.LogError("No names were found in the ‘m_Materials’ table. Testing alternative method...");
            TryAlternativeMethod(importer, projectMaterials, path);
        }
    }

    // If the materials table is empty, we search the sub-assets without filtering by type ‘Material’.
    void TryAlternativeMethod(ModelImporter importer, Dictionary<string, Material> projectMaterials, string path)
    {
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(path);
        int count = 0;

        foreach (var asset in allAssets)
        {
            if (asset == null) continue;

            // If it is not a Material, Unity may see it as a generic object 
            // but the name usually gives away whether it is a material slot
            string name = asset.name;
            string typeName = asset.GetType().Name;

            // 99% of the time, material slots in FBX contain the material name.
            // Try to clean it up and see if it matches the originals.
            string cleanName = Regex.Replace(name, @"(\.\d+)|(\s\(Instance\))$", "");

            if (projectMaterials.TryGetValue(cleanName, out Material targetMat))
            {
                var identifier = new AssetImporter.SourceAssetIdentifier(typeof(Material), name);
                importer.AddRemap(identifier, targetMat);
                count++;
            }
        }

        if (count > 0)
        {
            importer.SaveAndReimport();
            Debug.Log($"<color=green><b>Success!</b></color> Reassigned {count} materials.");
        }
    }
}