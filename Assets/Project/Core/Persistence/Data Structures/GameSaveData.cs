using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public InventorySaveData inventoryData = new();
    public List<string> collectedItemIDs = new();
}
