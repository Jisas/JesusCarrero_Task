using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldItem))]
public class WorldItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        WorldItem item = (WorldItem)target;

        if (string.IsNullOrEmpty(item.uniqueID))
        {
            // Si el ID está vacío, generamos uno automáticamente
            item.uniqueID = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(item);
        }

        if (GUILayout.Button("Regenerar ID Único"))
        {
            if (EditorUtility.DisplayDialog("Confirmación", "¿Seguro que quieres cambiar el ID? Esto reseteará su estado de guardado.", "Sí", "No"))
            {
                item.uniqueID = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(item);
            }
        }
    }
}