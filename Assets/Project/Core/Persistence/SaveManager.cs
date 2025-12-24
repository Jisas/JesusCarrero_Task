using UnityEngine;
using System.IO;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private GameData gameData = new();

    private string _savePath;
    private const string FileName = "save_slot_01.json";

    public void Initialize()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName);
    }


    // --- Centralized Persistence Logic ---
    public void SaveAllData(Transform playerTransform, InventorySO inventory, QuickSlotsSO quickSlots, CurrencySO currency)
    {
        gameData.playerData.position.x = playerTransform.position.x;
        gameData.playerData.position.y = playerTransform.position.y;
        gameData.playerData.position.z = playerTransform.position.z;
        gameData.playerData.rotation.x = playerTransform.rotation.x;
        gameData.playerData.rotation.y = playerTransform.rotation.y;
        gameData.playerData.rotation.z = playerTransform.rotation.z;
        gameData.playerData.rotation.w = playerTransform.rotation.w;
        gameData.inventoryData = inventory.GetSaveData();
        gameData.quickSlotsData = quickSlots.GetSaveData();
        gameData.playerData.selectedQuickSlotIndex = quickSlots.selectedIndex;
        gameData.playerData.currentGold = currency.TotalGold;

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(_savePath, json);

        Debug.Log("Game data saved");
    }
    public void ManualSaveAllData()
    {
        InventorySO inventory = ServiceLocator.Get<InventoryManager>().Inventory;
        QuickSlotsSO equipment = ServiceLocator.Get<QuickSlotsSO>();
        CurrencySO currency = ServiceLocator.Get<CurrencySO>();
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        SaveAllData(playerTransform, inventory, equipment, currency);
        Debug.Log("<color=green>SaveManager: Manually saved data.</color>");
    }
    public void LoadAllData(Transform playerTransform, InventorySO inventory, QuickSlotsSO quickSlots, ItemDatabaseSO database, CurrencySO currency)
    {
        if (!File.Exists(_savePath)) return;

        try
        {
            string json = File.ReadAllText(_savePath);
            gameData = JsonUtility.FromJson<GameData>(json);

            Vector3 savedPosition = new (
                gameData.playerData.position.x,
                gameData.playerData.position.y,
                gameData.playerData.position.z);

            Quaternion savedRotation = new (
                gameData.playerData.rotation.x,
                gameData.playerData.rotation.y,
                gameData.playerData.rotation.z,
                gameData.playerData.rotation.w);

            playerTransform.position = savedPosition == Vector3.zero ? playerTransform.position : savedPosition;
            playerTransform.rotation = savedRotation;
            inventory.LoadFromSaveData(gameData.inventoryData, database);
            quickSlots.LoadFromSaveData(gameData.quickSlotsData, database);
            quickSlots.selectedIndex = gameData.playerData.selectedQuickSlotIndex;
            currency.SetGold(gameData.playerData.currentGold);

            LoadDroppedItems(database);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }


    // --- Query methods for other systems ---
    public InventoryData GetInventoryData() => gameData.inventoryData;
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


    // --- Exclusive Editor's Block ---

    public void DeleteData()
    {
        _savePath = Path.Combine(Application.persistentDataPath, FileName);

        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.Log("All data was delete");
        }

#if UNITY_EDITOR 
        CleanAllScriptableObjects(); // Exclusive Editor's Call
#endif
    }

#if UNITY_EDITOR
    private void CleanAllScriptableObjects()
    {
        InventorySO inventory = null;
        QuickSlotsSO quickSlots = null;
        CurrencySO currency = null;

        if (Application.isPlaying)
        {
            var invManager = ServiceLocator.Get<InventoryManager>();
            quickSlots = ServiceLocator.Get<QuickSlotsSO>();
            currency = ServiceLocator.Get<CurrencySO>();

            if (invManager != null) inventory = invManager.Inventory;

        }
        else
        {
            inventory = FindAssetByType<InventorySO>();
            quickSlots = FindAssetByType<QuickSlotsSO>();
            currency= FindAssetByType<CurrencySO>();
        }

        if (inventory != null)
        {
            inventory.ClearInventory();
            Debug.Log("<color=yellow>SaveManager: InventorySO reset.</color>");
        }

        if (quickSlots != null)
        {
            quickSlots.ClearEquipment();
            Debug.Log("<color=yellow>SaveManager: QuickSlotsSO reset.</color>");
        }

        if (currency != null)
        {
            currency.SetGold(0);
        }

        UnityEditor.AssetDatabase.SaveAssets();
    }

    // Helper
    private T FindAssetByType<T>() where T : UnityEngine.Object
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return null;
    }
#endif
}