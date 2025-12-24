using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public PlayerData playerData = new();
    public InventoryData inventoryData = new();
    public InventoryData quickSlotsData = new();
    public List<string> collectedItemIDs = new();
    public List<DroppedItemData> droppedItems = new();
}
