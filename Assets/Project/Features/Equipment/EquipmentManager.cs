using UnityEngine;

public class EquipmentManager : MonoBehaviour, IInitializable
{
    private QuickSlotsSO quickSlotsData;
    private QuickSlotDisplay equipmentDisplay;
    private InputReaderSO inputReader;

    public void Initialize()
    {
        quickSlotsData = ServiceLocator.Get<QuickSlotsSO>();
        equipmentDisplay = ServiceLocator.Get<QuickSlotDisplay>();
        inputReader = ServiceLocator.Get<InputReaderSO>();

        inputReader.Previous += ChangeQuickSlot;
        inputReader.Next += ChangeQuickSlot;
        inputReader.UseItem += UseEquippedItem;

        equipmentDisplay.UpdateUI(quickSlotsData.selectedIndex);
    }

    private void ChangeQuickSlot(int direction)
    {
        // Add the direction (-1 or 1) to the current index
        int newIndex = quickSlotsData.selectedIndex + direction;

        // Circular logic for 3 slots (0, 1, 2)
        if (newIndex < 0)
            newIndex = quickSlotsData.equippedItems.Count - 1;
        else if (newIndex >= quickSlotsData.equippedItems.Count)
            newIndex = 0;

        quickSlotsData.selectedIndex = newIndex;

        // Update the UI by passing the newly selected index.
        equipmentDisplay.UpdateUI(quickSlotsData.selectedIndex);

        // Optional: “Tick” sound here for immediate feedback
    }

    private void UseEquippedItem()
    {
        var selected = quickSlotsData.equippedItems[quickSlotsData.selectedIndex];
        if (selected.isEmpty) return;

        selected.item.Use();
        selected.amount--;

        if (selected.amount <= 0) selected.Clear();

        equipmentDisplay.UpdateUI(quickSlotsData.selectedIndex);
    }

    private void OnDestroy()
    {
        inputReader.Previous -= ChangeQuickSlot;
        inputReader.Next -= ChangeQuickSlot;
        inputReader.UseItem -= UseEquippedItem;
    }
}
