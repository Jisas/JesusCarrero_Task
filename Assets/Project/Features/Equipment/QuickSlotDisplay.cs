using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class QuickSlotDisplay : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private QuickSlotsSO data;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI textAmount;
    [SerializeField] private TextMeshProUGUI textName;

    public void Initialize()
    {
        data.OnQuickSlotsUpdated += UpdateUI;
    }

    public void UpdateUI(int id)
    {
        icon.gameObject.SetActive(!data.equippedItems[id].isEmpty);

        if (!data.equippedItems[id].isEmpty)
        {
            icon.enabled = true;
            icon.sprite = data.equippedItems[id].item.icon;
            textAmount.text = data.equippedItems[id].amount.ToString();
            textName.text = data.equippedItems[id].item.itemName;
        }
        else
        {
            icon.enabled = false;
            icon.sprite = null;
            textAmount.text = string.Empty;
            textName.text = string.Empty;
        }
    }

    private void OnDestroy()
    {
        data.OnQuickSlotsUpdated -= UpdateUI;
    }
}