using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemData;
    [SerializeField] private int amount = 1;

    public InteractionType Type => InteractionType.Pickup;
    public string InteractionPrompt => $"Pickup {itemData.itemName}";

    public void Interact(PlayerController player)
    {
        // We access the inventory through the ServiceLocator
        var inventory = ServiceLocator.Get<InventoryManager>().Inventory;

        if (inventory.AddItem(itemData, amount))
        {
            Destroy(gameObject);
        }
    }
}