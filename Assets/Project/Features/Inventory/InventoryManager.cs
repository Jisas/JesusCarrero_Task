using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private InventorySO inventory;
    [SerializeField] private QuickSlotsSO equipment;
    [SerializeField] private InventoryDisplay inventoryDisplay;

    public InventorySO Inventory => inventory;

    public void Initialize()
    {
        inventory.Initialize();
        inventoryDisplay.Initialize(inventory, equipment);
    }
}