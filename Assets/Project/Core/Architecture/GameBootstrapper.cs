using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private ItemDatabaseSO itemDB;
    [SerializeField] private QuickSlotsSO quickSlots;

    [Header("Monobehaviours")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private SettingsManager settingsManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueDisplay dialogueDisplay;
    [SerializeField] private QuickSlotDisplay equipmentDislay;

    private void Awake()
    {
        // 1. Service registration
        ServiceLocator.Register<ItemDatabaseSO>(itemDB);
        ServiceLocator.Register<InputReaderSO>(inputReader);
        ServiceLocator.Register<QuickSlotsSO>(quickSlots);
        ServiceLocator.Register<SaveManager>(saveManager);
        ServiceLocator.Register<SettingsManager>(settingsManager);
        ServiceLocator.Register<PlayerController>(playerController);
        ServiceLocator.Register<PlayerStats>(playerStats);
        ServiceLocator.Register<InventoryManager>(inventoryManager);
        ServiceLocator.Register<DialogueManager>(dialogueManager);
        ServiceLocator.Register<QuickSlotDisplay>(equipmentDislay);

        // 2. Ordered initialization
        saveManager.Initialize();
        quickSlots.Initialize();
        inventoryManager.Initialize();

        saveManager.LoadAllData(inventoryManager.Inventory, quickSlots, itemDB);

        settingsManager.Initialize();
        inputReader.Initialize();
        equipmentManager.Initialize();
        equipmentDislay.Initialize();
        playerController.Initialize();
        playerStats.Initialize();
        dialogueDisplay.Initialize();

        // 3. Notify that the game is ready
        Debug.Log("Systems successfully initialized.");
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveAllData(inventoryManager.Inventory, quickSlots);
    }
}
