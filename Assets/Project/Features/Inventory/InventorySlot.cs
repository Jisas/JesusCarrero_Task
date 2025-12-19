using UnityEngine.UI;
using TMPro;

public class InventorySlot : BaseSlot, IVisualSlot
{
    public Image itemImage;
    public TextMeshProUGUI amountText;

    public void SetUp(InventorySlotInfo data)
    {
        SlotInfo = data;
    }

    public void UpdateUI()
    {
        if (SlotInfo.isEmpty)
        {
            itemImage.sprite = null;
            amountText.text = string.Empty;
            itemImage.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
        else
        {
            var currentItem = SlotInfo.item;
            var itemIcon = currentItem.icon;
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = itemIcon;

            if (SlotInfo.amount > 1)
            {
                amountText.text = SlotInfo.amount.ToString();
                amountText.gameObject.SetActive(true);
            }
            else
            {
                amountText.text = string.Empty;
                amountText.gameObject.SetActive(false);
            }
        }
    }
}
