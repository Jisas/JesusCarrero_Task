using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour, IInitializable, IGameService
{
    private string _savePath;
    private const string FileName = "inventory_data.json";

    public void Initialize()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName);
    }

    public void SaveInventory(InventorySO inventory)
    {
        InventorySaveData data = inventory.GetSaveData();
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Inventario guardado en: {_savePath}");
    }

    public bool LoadInventory(InventorySO inventory, ItemDatabaseSO database)
    {
        if (!File.Exists(_savePath))
        {
            Debug.LogWarning("No se encontró archivo de guardado.");
            return false;
        }

        try
        {
            string json = File.ReadAllText(_savePath);
            InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
            inventory.LoadFromSaveData(data, database);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error al cargar: {e.Message}");
            return false;
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(_savePath)) File.Delete(_savePath);
    }
}