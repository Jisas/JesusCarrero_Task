using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public SettingsData settingsData = new();
    [Space(10)]
    public InventoryData inventoryData = new();
    [Space(10)]
    public int selectedQuickSlotIndex = 0;
    public InventoryData quickSlotsData = new();
    [Space(10)]
    public List<string> collectedItemIDs = new();
    [Space(10)]
    public List<DroppedItemData> droppedItems = new();
}
