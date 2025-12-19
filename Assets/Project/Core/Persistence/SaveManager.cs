using UnityEngine;
using System.IO;
using static Unity.VisualScripting.LudiqRootObjectEditor;
using UnityEngine.Splines.ExtrusionShapes;

public class SaveManager : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private GameSaveData gameData = new();

    private string _savePath;
    private const string FileName = "save_slot_01.json";

    public void Initialize()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName);
    }

    // --- Centralized Persistence Logic ---

    public void SaveAllData(InventorySO inventory)
    {
        // Update the gameData object before saving
        gameData.inventoryData = inventory.GetSaveData();

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(_savePath, json);
        Debug.Log($"Game data saved at: {_savePath}");
    }

    public void LoadAllData(InventorySO inventory, ItemDatabaseSO database)
    {
        if (!File.Exists(_savePath)) return;

        try
        {
            string json = File.ReadAllText(_savePath);
            gameData = JsonUtility.FromJson<GameSaveData>(json);

            // Individual inventory load
            inventory.LoadFromSaveData(gameData.inventoryData, database);

            Debug.Log("Game Data Load.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }

    // --- Query methods for other systems ---

    public bool IsItemCollected(string id) => gameData.collectedItemIDs.Contains(id);

    public void RegisterPickup(string id)
    {
        if (!gameData.collectedItemIDs.Contains(id))
        {
            gameData.collectedItemIDs.Add(id);
        }
    }

    public InventorySaveData GetInventoryData() => gameData.inventoryData;

    public void SaveFromEditor()
    {
        InventorySO inventory = ServiceLocator.Get<InventoryManager>().Inventory;
        SaveAllData(inventory);
        Debug.Log("<color=green>SaveManager: Manually saved data.</color>");
    }
    public void DeleteData()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName);

        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.Log("All data was delete");
        }

#if UNITY_EDITOR 
        CleanInventorySO(); // Exclusive Editor's Call
#endif
    }

    // --- Exclusive Editor's Block ---

#if UNITY_EDITOR
    private void CleanInventorySO()
    {
        InventorySO inventory = null;

        if (Application.isPlaying)
        {
            // En tiempo de ejecución usamos el ServiceLocator
            var invManager = ServiceLocator.Get<InventoryManager>();
            if (invManager != null) inventory = invManager.Inventory;
        }
        else
        {
            // En Modo Edición, buscamos el Asset directamente en la base de datos del proyecto
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:InventorySO");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                inventory = UnityEditor.AssetDatabase.LoadAssetAtPath<InventorySO>(path);
            }
        }

        if (inventory != null)
        {
            inventory.ClearInventory();
            Debug.Log("<color=yellow>SaveManager: Inventory ScriptableObject reset successfully.</color>");
        }
        else
        {
            Debug.LogWarning("SaveManager: No se pudo encontrar el InventorySO para limpiar.");
        }
    }
#endif
}