using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class SaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Dibuja el inspector por defecto (para ver la lista de IDs, etc)
        DrawDefaultInspector();

        SaveManager manager = (SaveManager)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Persistence Controls", EditorStyles.boldLabel);

        // Botón de Guardar
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Save Current Game", GUILayout.Height(30)))
        {
            if (Application.isPlaying)
                manager.SaveFromEditor();
            else
                Debug.LogWarning("It must be in Play mode to save the current status.");
        }

        // Botón de Borrar
        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("DELETE EVERYTHING (File and OS)", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Critical Confirmation",
                "Are you sure you want to delete the JSON file and reset the inventory? This action cannot be undone.",
                "Yes", "Cancel"))
            {
                manager.DeleteData();
            }
        }

        GUI.backgroundColor = Color.white;
    }
}