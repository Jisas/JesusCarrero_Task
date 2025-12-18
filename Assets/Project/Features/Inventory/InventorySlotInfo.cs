[System.Serializable]
public class InventorySlotInfo
{
    public ItemSO item;
    public int amount;
    public bool isEmpty;

    public InventorySlotInfo(ItemSO item, int amount, bool isEmpty)
    {
        this.item = item;
        this.amount = amount;
        this.isEmpty = isEmpty;
    }

    public void Clear()
    { 
        item = null;
        amount = 0;
        isEmpty = true;
    }
}
