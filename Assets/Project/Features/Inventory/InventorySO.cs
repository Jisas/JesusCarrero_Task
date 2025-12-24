using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInventory", menuName = "Project/Inventory")]
public class InventorySO : ScriptableObject
{
    public int capacity = 20;
    [Space(5)]
    public List<InventorySlotInfo> slots = new();

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
    public InventoryData GetSaveData()
    {
        var data = new InventoryData();
        foreach (var slot in slots)
        {
            data.slots.Add(new SlotData
            {
                itemID = slot.isEmpty ? string.Empty : slot.item.ID,
                amount = slot.amount,
                isEmpty = slot.isEmpty
            });
        }
        return data;
    }
    public void LoadFromSaveData(InventoryData data, ItemDatabaseSO database)
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
    public void RemoveItem(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        slots[index].Clear();
        NotifyUpdated();
    }
    public void SwapSlots(int indexA, int indexB)
    {
        (slots[indexB], slots[indexA]) = (slots[indexA], slots[indexB]);
        OnInventoryUpdated?.Invoke();
    }
    public void DropItem(int index)
    {
        if (index < 0 || index >= slots.Count) return;

        slots[index].Clear();
        OnInventoryUpdated?.Invoke(); // Aquí sí tienes permiso para invocarlo
    }
    public void UseItem(int index)
    {
        var slot = slots[index];
        if (slot.isEmpty) return;
        
        slot.item.Use(); // Execute item logic

        if (slot.item.isStackable && slot.amount > 1)
        {
            slot.amount--;
        }
        else
        {
            slot.Clear();
        }

        OnInventoryUpdated?.Invoke();
    }
    public void NotifyUpdated()
    {
        OnInventoryUpdated?.Invoke();
    }
    public bool HasSpace(ItemSO item, int amount = 1)
    {
        // 1. If the item is stackable, check if there is a slot with the same item that has space.
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                // If the slot has the same item and the total quantity does not exceed the maximum stack, proceed.
                if (!slot.isEmpty && slot.item == item && (slot.amount + amount) <= item.maxStack)
                {
                    return true;
                }
            }
        }

        // 2. If it cannot be stacked (or there is no space in the stacks), look for an empty slot.
        return slots.Exists(s => s.isEmpty);
    }


#if UNITY_EDITOR
    /// <summary>
    /// Resets the slots to their empty state
    /// </summary>
    public void ClearInventory()
    {
        slots.Clear();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets(); // Force Unity to save the change to disk
    }
#endif
}