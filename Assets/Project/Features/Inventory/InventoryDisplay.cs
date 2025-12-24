using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventoryDisplay : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotsPanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("Drag & Drop Visuals")]
    [SerializeField] private Image ghostIcon;

    private InventorySO _inventory;
    private QuickSlotsSO _equipment;
    private readonly List<InventorySlot> _uiSlots = new();

    // --- General Methods ---

    public void Initialize(InventorySO inventory, QuickSlotsSO equipment)
    {
        _inventory = inventory;
        _equipment = equipment;

        ProjectExtensions.OnInventoryRequest += HandleDisplay;
        _inventory.OnInventoryUpdated += RefreshUI;

        inventoryPanel.SetActive(false);
        ghostIcon.gameObject.SetActive(false);

        for (int i = 0; i < inventory.capacity; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, slotsPanel);
            InventorySlot newSlot = slotGO.GetComponent<InventorySlot>();

            // Pass the inventory reference so that the slot knows who to ask for the Swap
            newSlot.SetUp(i, this, inventory.slots[i]);
            _uiSlots.Add(newSlot);
            newSlot.UpdateUI();
        }
    }
    private void HandleDisplay(bool shouldShow)
    {
        inventoryPanel.SetActive(shouldShow);

        if (shouldShow)
        {
            // Select the first slot for controller/keyboard support
            var firstSlot = slotsPanel.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(firstSlot);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
    public void RefreshUI()
    {
        for (int i = 0; i < _uiSlots.Count; i++)
        {
            // Resynchronize the UI Slot with the OS Data Slot
            _uiSlots[i].RefreshSlotData(_inventory.slots[i]);
        }
    }


    // --- Data Display Methods ---

    public void Show(ItemSO item)
    {
        nameText.text = item.itemName;
        descriptionText.text = item.description;
    }
    public void Hide()
    {
        nameText.text = string.Empty;
        descriptionText.text = string.Empty;
    }


    // --- Methods for Drag & Drop ---

    public void StartDragging(Sprite icon)
    {
        ghostIcon.sprite = icon;
        ghostIcon.gameObject.SetActive(true);
    }
    public void UpdateDragging(Vector2 position) => ghostIcon.transform.position = position;
    public void StopDragging() => ghostIcon.gameObject.SetActive(false);
    public void RequestSwap(int indexA, int indexB)
    {
        _inventory.SwapSlots(indexA, indexB);
    }
    public void RequestDrop(int index)
    {
        var slotInfo = _inventory.slots[index];
        if (slotInfo.isEmpty) return;

        // Visual logic/world: Physically drop the object
        var player = ServiceLocator.Get<PlayerController>();
        Vector3 objectRelativePos = new (player.transform.position.x, 0.7f, player.transform.position.z);
        Vector3 dropPos = objectRelativePos + player.transform.forward * 1.5f;

        
        if (slotInfo.item.worldPrefab != null)
        {
            // 1. Instantiate
            GameObject droppedObj = Instantiate(slotInfo.item.worldPrefab, dropPos, Quaternion.identity);

            // 2. Generate/Maintain ID
            string uID = System.Guid.NewGuid().ToString();
            droppedObj.GetComponent<WorldItem>().uniqueID = uID;

            // 3. Persistence
            ServiceLocator.Get<SaveManager>().RegisterDroppedItem(slotInfo.item, dropPos, uID);
        }

        // Data logic: The OS cleans itself and notifies
        _inventory.DropItem(index);
    }


    // --- Methods for equip items ---
    public void RequestEquipItem(int index)
    {
        var slot = _inventory.slots[index];
        if (slot.isEmpty) return;

        if (slot.item.type == ItemType.Consumable)
        {
            bool equipped = _equipment.TryEquip(slot.item, slot.amount);

            if (equipped)
            {
                _inventory.RemoveItem(index);
                Hide();
            }
            else
            {
                Debug.LogWarning("No hay espacio en los QuickSlots");
            }
        }
    }


    // --- Monobehaviour ---

    private void OnDestroy()
    {
        ProjectExtensions.OnInventoryRequest -= HandleDisplay;
        _inventory.OnInventoryUpdated -= RefreshUI;
    }
}
