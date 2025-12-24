using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private ItemDatabaseSO itemDB;
    [SerializeField] private QuickSlotsSO quickSlots;
    [SerializeField] private CurrencySO currency;

    [Header("Monobehaviours")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerInteractor playerInteractor;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private SaleManager saleManager;
    [Space(10)]
    [SerializeField] private MenuDisplay menuDisplay;
    [SerializeField] private DialogueDisplay dialogueDisplay;
    [SerializeField] private QuickSlotDisplay equipmentDislay;
    [SerializeField] private InteractorDisplay interactorDisplay;
    [SerializeField] private CurrencyDisplay currencyDisplay;
    [SerializeField] private VendorDisplay vendorDisplay;

    private void Awake()
    {
        // 1. Service registration
        ServiceLocator.Register<ItemDatabaseSO>(itemDB);
        ServiceLocator.Register<InputReaderSO>(inputReader);
        ServiceLocator.Register<QuickSlotsSO>(quickSlots);
        ServiceLocator.Register<SaveManager>(saveManager);
        ServiceLocator.Register<PlayerController>(playerController);
        ServiceLocator.Register<PlayerStats>(playerStats);
        ServiceLocator.Register<PlayerInteractor>(playerInteractor);
        ServiceLocator.Register<InventoryManager>(inventoryManager);
        ServiceLocator.Register<DialogueManager>(dialogueManager);
        ServiceLocator.Register<CurrencySO>(currency);
        ServiceLocator.Register<SaleManager>(saleManager);
        ServiceLocator.Register<QuickSlotDisplay>(equipmentDislay);
        ServiceLocator.Register<VendorDisplay>(vendorDisplay);

        // 2. Ordered initialization
        saveManager.Initialize();
        quickSlots.Initialize();
        inventoryManager.Initialize();
        currencyDisplay.Initialize();

        saveManager.LoadAllData(inventoryManager.Inventory, quickSlots, itemDB, currency);

        inputReader.Initialize();
        menuDisplay.Initialize();
        equipmentManager.Initialize();
        equipmentDislay.Initialize();
        playerController.Initialize();
        playerStats.Initialize();
        interactorDisplay.Initialize();
        dialogueDisplay.Initialize();

        // 3. Notify that the game is ready
        Debug.Log("Systems successfully initialized.");
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveAllData(inventoryManager.Inventory, quickSlots, currency);
    }

    public void QuitGame() => Application.Quit();
}
