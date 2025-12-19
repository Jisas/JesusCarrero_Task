using UnityEngine.EventSystems;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsPanel;

    public void Initialize(InventorySO inventory)
    {
        // 1. Subscription to events
        InventoryEvents.OnInventoryRequest += HandleDisplay;
        inventoryPanel.SetActive(false);

        // 2. Build inventory UI
        for (int i = 0; i < inventory.capacity; i++)
        {
            GameObject slot = Instantiate<GameObject>(slotPrefab, slotsPanel);
            InventorySlot newSlot = slot.GetComponent<InventorySlot>();
            newSlot.SetUp(inventory.slots[i]);
            newSlot.UpdateUI();
        }
    }

    private void HandleDisplay(bool shouldShow)
    {
        Debug.Log("Entre");
        inventoryPanel.SetActive(shouldShow);

        if (shouldShow)
        {
            // Seleccionamos el primer slot para soporte de mando/teclado
            var firstSlot = slotsPanel.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(firstSlot);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    private void OnDestroy()
    {
        InventoryEvents.OnInventoryRequest -= HandleDisplay;
    }
}
