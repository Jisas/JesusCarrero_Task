using UnityEngine;
using System.IO;

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
    public void SaveAllData(InventorySO inventory, QuickSlotsSO quickSlots)
    {
        gameData.inventoryData = inventory.GetSaveData();
        gameData.quickSlotsData = quickSlots.GetSaveData();
        gameData.selectedQuickSlotIndex = quickSlots.selectedIndex;

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(_savePath, json);

        Debug.Log("Game data saved");
    }
    public void LoadAllData(InventorySO inventory, QuickSlotsSO quickSlots, ItemDatabaseSO database)
    {
        if (!File.Exists(_savePath)) return;

        try
        {
            string json = File.ReadAllText(_savePath);
            gameData = JsonUtility.FromJson<GameData>(json);

            inventory.LoadFromSaveData(gameData.inventoryData, database);
            quickSlots.LoadFromSaveData(gameData.quickSlotsData, database);
            quickSlots.selectedIndex = gameData.selectedQuickSlotIndex;

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
    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SettingsData>(json);
        }
        else
        {
            Resolution[] resolutions = Screen.resolutions;
            Resolution currentResolution = Screen.currentResolution;
            int currentResolutionIndex = -1;

            foreach (var res in resolutions)
            {
                if (res.width == currentResolution.width && res.height == currentResolution.height)
                    currentResolutionIndex = resolutions.IndexOfItem(res);
            }

            return new SettingsData()
            {
                Quality = 0,
                ScreenResolution = currentResolutionIndex,
                ScreenMode = 0,
                TextureResolution = 0,
                ShadowQuality = 2,
                ShadowResolution = 2,
                RenderScale = 1,
                FrameRate = 2,
                Brightness = 1,
                AntiAliasing = true,

                GeneralVolume = 1,
                MusicVolume = 1,
                AmbientalVolume = 1,
                EffectsVolume = 1,
                UIVolume = 0.1f,
            };
        }
    }
    public SettingsData GetSettingsData() => gameData.settingsData;


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
    public void SaveFromEditor()
    {
        InventorySO inventory = ServiceLocator.Get<InventoryManager>().Inventory;
        QuickSlotsSO equipment = ServiceLocator.Get<QuickSlotsSO>();

        SaveAllData(inventory, equipment);
        Debug.Log("<color=green>SaveManager: Manually saved data.</color>");
    }

    private void CleanAllScriptableObjects()
    {
        InventorySO inventory = null;
        QuickSlotsSO quickSlots = null;

        if (Application.isPlaying)
        {
            var invManager = ServiceLocator.Get<InventoryManager>();
            if (invManager != null) inventory = invManager.Inventory;

            quickSlots = ServiceLocator.Get<QuickSlotsSO>();
        }
        else
        {
            inventory = FindAssetByType<InventorySO>();
            quickSlots = FindAssetByType<QuickSlotsSO>();
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