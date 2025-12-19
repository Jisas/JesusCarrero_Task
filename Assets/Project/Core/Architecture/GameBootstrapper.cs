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
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueDisplay dialogueDisplay;

    private void Awake()
    {
        // 1. Service registration
        ServiceLocator.Register<SaveManager>(saveManager);
        ServiceLocator.Register<InputReaderSO>(inputReader);
        ServiceLocator.Register<InventoryManager>(playerInventory);
        ServiceLocator.Register<PlayerController>(playerController);
        ServiceLocator.Register<DialogueManager>(dialogueManager);

        // 2. Ordered initialization
        saveManager.Initialize();
        playerInventory.Initialize();

        bool success = saveManager.LoadInventory(playerInventory.Inventory, itemDB);

        if (success) Debug.Log("Inventory loaded from file.");
        else Debug.Log("Starting with new inventory.");

        inputReader.Initialize();
        playerController.Initialize();
        dialogueDisplay.Initialize();

        // 3. Notify that the game is ready
        Debug.Log("Systems successfully initialized.");
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveInventory(playerInventory.Inventory);
    }
}
