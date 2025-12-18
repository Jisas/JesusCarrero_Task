using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private InputReaderSO inputReader;
    [SerializeField] private ItemDatabaseSO itemDB;

    [Header("Monobehaviours")]
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private InventoryManager playerInventory;

    private void Awake()
    {
        // 1. Service registration
        ServiceLocator.Register<SaveManager>(saveManager);
        ServiceLocator.Register<InputReaderSO>(inputReader);
        ServiceLocator.Register<InventoryManager>(playerInventory);
        ServiceLocator.Register<PlayerController>(playerController);

        // 2. Ordered initialization
        saveManager.Initialize();
        playerInventory.Initialize();

        bool success = saveManager.LoadInventory(playerInventory.Inventory, itemDB);

        if (success) Debug.Log("Inventario cargado desde archivo.");
        else Debug.Log("Iniciando con inventario nuevo.");

        inputReader.Initialize();
        playerController.Initialize();

        // 3. Notify that the game is ready
        Debug.Log("Sistemas inicializados con éxito.");
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveInventory(playerInventory.Inventory);
    }
}
