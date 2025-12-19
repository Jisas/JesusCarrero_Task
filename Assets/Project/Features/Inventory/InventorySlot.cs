using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class InventorySlot : BaseSlot, IVisualSlot, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;

    private int _slotIndex;
    private InventoryDisplay _display;

    // ----- General -----

    public void SetUp(int index, InventoryDisplay display, InventorySlotInfo data)
    {
        _slotIndex = index;
        _display = display;
        SlotInfo = data;
    }

    public void RefreshSlotData(InventorySlotInfo newData)
    {
        SlotInfo = newData;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (SlotInfo.isEmpty)
        {
            _icon.sprite = null;
            _amountText.text = string.Empty;
            _icon.gameObject.SetActive(false);
            _amountText.gameObject.SetActive(false);
        }
        else
        {
            var currentItem = SlotInfo.item;
            var itemIcon = currentItem.icon;
            _icon.gameObject.SetActive(true);
            _icon.sprite = itemIcon;

            if (SlotInfo.amount > 1)
            {
                _amountText.text = SlotInfo.amount.ToString();
                _amountText.gameObject.SetActive(true);
            }
            else
            {
                _amountText.text = string.Empty;
                _amountText.gameObject.SetActive(false);
            }
        }
    }


    // ----- Interface Implementations -----

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (SlotInfo.isEmpty) return;

        _display.StartDragging(_icon.sprite);

        // Makes the original icon slightly transparent while dragging
        _icon.color = new Color(1, 1, 1, 0.5f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _display.UpdateDragging(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _display.StopDragging();
        _icon.color = Color.white;

        // If we drop it outside of any UI (outside of the inventory)
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            _display.RequestDrop(_slotIndex);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<InventorySlot>(out var sourceSlot))
        {
            _display.RequestSwap(sourceSlot._slotIndex, this._slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SlotInfo.isEmpty) return;
        _display.Show(SlotInfo.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _display.Hide();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (SlotInfo.isEmpty)
        {
            _display.Hide();
            return;
        }
        _display.Show(SlotInfo.item);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        _display.Hide();
    }
}
