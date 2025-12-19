using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemData;
    [SerializeField] private int amount = 1;

    public InteractionType Type => InteractionType.Pickup;
    public string InteractionPrompt => $"Pickup {itemData.itemName}";

    public void Interact(PlayerController player)
    {
        var inventory = ServiceLocator.Get<InventoryManager>().Inventory;

        if (inventory.AddItem(itemData, amount))
        {
            WorldItem info = GetComponent<WorldItem>();
            ServiceLocator.Get<SaveManager>().UnregisterWorldItem(info.uniqueID, info.isDynamic);
            Destroy(gameObject);
        }
    }
}