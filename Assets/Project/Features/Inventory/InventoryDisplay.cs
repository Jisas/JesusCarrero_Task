using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform slotsPanel;

    public void Initialize(InventorySO inventory)
    {
        for (int i = 0; i < inventory.capacity; i++)
        {
            GameObject slot = Instantiate<GameObject>(slotPrefab, slotsPanel);
            InventorySlot newSlot = slot.GetComponent<InventorySlot>();
            newSlot.SetUp(inventory.slots[i]);
            newSlot.UpdateUI();
        }
    }
}
