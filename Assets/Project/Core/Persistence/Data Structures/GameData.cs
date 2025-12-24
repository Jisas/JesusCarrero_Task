using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentGold = 5000;
    public int selectedQuickSlotIndex = 0;
    [Space(10)]
    public InventoryData inventoryData = new();
    public InventoryData quickSlotsData = new();
    public List<string> collectedItemIDs = new();
    public List<DroppedItemData> droppedItems = new();
}
