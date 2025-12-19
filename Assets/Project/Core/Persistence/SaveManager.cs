using UnityEngine;
using System.IO;

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
        Debug.Log($"Game data saved");
    }

    public void LoadAllData(InventorySO inventory, ItemDatabaseSO database)
    {
        if (!File.Exists(_savePath)) return;

        try
        {
            string json = File.ReadAllText(_savePath);
            gameData = JsonUtility.FromJson<GameSaveData>(json);

            // 1. Load inventory
            inventory.LoadFromSaveData(gameData.inventoryData, database);

            // 2. Spawning items that were on the ground
            LoadDroppedItems(database);

            Debug.Log("Game Data Load.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }


    // --- Query methods for other systems ---
    public InventorySaveData GetInventoryData() => gameData.inventoryData;

    public bool IsItemCollected(string id) => gameData.collectedItemIDs.Contains(id);

    public void UnregisterWorldItem(string id, bool isDynamic)
    {
        if (isDynamic)
        {
            // If the object was temporary (thrown by the player), 
            // we remove it from the list of existing objects on the ground.
            gameData.droppedItems.RemoveAll(i => i.uniqueID == id);
            Debug.Log($"Item dinámico {id} removido de la persistencia.");
        }
        else
        {
            // If the object was originally on the map, we mark it as 
            // “permanently collected.”
            if (!gameData.collectedItemIDs.Contains(id))
            {
                gameData.collectedItemIDs.Add(id);
                Debug.Log($"Static item {id} marked as collected.");
            }
        }
    }

    public void RegisterDroppedItem(ItemSO item, Vector3 position, string uID)
    {
        gameData.droppedItems.Add(new DroppedItemData
        {
            itemID = item.ID,
            uniqueID = uID,
            position = position
        });
    }

    public void LoadDroppedItems(ItemDatabaseSO database)
    {
        foreach (var data in gameData.droppedItems)
        {
            ItemSO item = database.GetItemByID(data.itemID);
            if (item == null || item.worldPrefab == null) continue;

            GameObject go = Instantiate(item.worldPrefab, data.position, Quaternion.identity);
            WorldItem worldItem = go.GetComponent<WorldItem>();

            if (worldItem != null)
            {
                worldItem.uniqueID = data.uniqueID;
                worldItem.isDynamic = true;
            }
        }
    }

    public void RemoveDroppedItem(string uID)
    {
        gameData.droppedItems.RemoveAll(i => i.uniqueID == uID);
    }

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