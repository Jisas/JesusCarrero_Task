using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "QuickSlots", menuName = "Project/Equipment/QuickSlots")]
public class QuickSlotsSO : ScriptableObject, IInitializable, IGameService
{
    public List<InventorySlotInfo> equippedItems = new();
    public int selectedIndex = 0;

    public event Action<int> OnQuickSlotsUpdated;

    public void Initialize()
    {
        for (int i = 0; i < 3; i++)
            if (equippedItems.Count < 3) 
                equippedItems.Add(new InventorySlotInfo(null, 0, true));
    }

    public InventoryData GetSaveData()
    {
        InventoryData data = new InventoryData();

        foreach (var slot in equippedItems)
        {
            data.slots.Add(new SlotData
            {
                itemID = slot.isEmpty ? "" : slot.item.ID,
                amount = slot.amount,
                isEmpty = slot.isEmpty
            });
        }
        return data;
    }

    public void LoadFromSaveData(InventoryData data, ItemDatabaseSO database)
    {
        for (int i = 0; i < equippedItems.Count; i++)
        {
            if (i < data.slots.Count && !data.slots[i].isEmpty)
            {
                ItemSO item = database.GetItemByID(data.slots[i].itemID);
                equippedItems[i].item = item;
                equippedItems[i].amount = data.slots[i].amount;
                equippedItems[i].isEmpty = false;
            }
            else
            {
                equippedItems[i].Clear();
            }
        }
    }

    public bool TryEquip(ItemSO item, int amount)
    {
        for (int i = 0; i < equippedItems.Count; i++)
        {
            if (equippedItems[i].isEmpty)
            {
                equippedItems[i].item = item;
                equippedItems[i].amount = amount;
                equippedItems[i].isEmpty = false;
                OnQuickSlotsUpdated?.Invoke(i);
                return true; 
            }
        }
        return false;
    }

    public void ClearEquipment()
    {
        equippedItems.Clear();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets(); // Force Unity to save the change to disk
    }
}