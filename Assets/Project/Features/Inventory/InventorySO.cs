using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Project/Inventory")]
public class InventorySO : ScriptableObject
{
    public List<InventorySlotInfo> slots = new();
    public int capacity = 20;

    public event System.Action OnInventoryUpdated;

    public void Initialize()
    {
        if (slots.Count == capacity) return;

        slots.Clear();

        for (int i = 0; i < capacity; i++)
        {
            slots.Add(new InventorySlotInfo(null, 0, true));
        }
    }

    public InventorySaveData GetSaveData()
    {
        var data = new InventorySaveData();
        foreach (var slot in slots)
        {
            data.slots.Add(new SlotSaveData
            {
                itemID = slot.isEmpty ? string.Empty : slot.item.ID,
                amount = slot.amount,
                isEmpty = slot.isEmpty
            });
        }
        return data;
    }

    public void LoadFromSaveData(InventorySaveData data, ItemDatabaseSO database)
    {
        // Ensure that capacity matches demand.
        for (int i = 0; i < capacity; i++)
        {
            if (i >= data.slots.Count) break;

            var savedSlot = data.slots[i];
            if (savedSlot.isEmpty || string.IsNullOrEmpty(savedSlot.itemID))
            {
                slots[i].Clear();
            }
            else
            {
                slots[i].item = database.GetItemByID(savedSlot.itemID);
                slots[i].amount = savedSlot.amount;
                slots[i].isEmpty = false;
            }
        }
        OnInventoryUpdated?.Invoke();
    }

    public bool AddItem(ItemSO item, int amount)
    {
        // 1. Attempting to stack in existing slots
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (!slot.isEmpty && slot.item == item && slot.amount < item.maxStack)
                {
                    slot.amount += amount;
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
        }

        // 2. Find the first slot marked as isEmpty
        var emptySlot = slots.Find(s => s.isEmpty);
        if (emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.amount = amount;
            emptySlot.isEmpty = false;
            OnInventoryUpdated?.Invoke();
            return true;
        }

        return false;
    }

    public void SwapSlots(int indexA, int indexB)
    {
        (slots[indexB], slots[indexA]) = (slots[indexA], slots[indexB]);
        OnInventoryUpdated?.Invoke();
    }
}