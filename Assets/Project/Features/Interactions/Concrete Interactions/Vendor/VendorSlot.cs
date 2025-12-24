using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class VendorSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public Button buyButton;

    public void Setup(CurrencySO playerCurrency, ItemSO item, Action<CurrencySO, ItemSO> onBuyAction)
    {
        icon.sprite = item.icon;
        itemName.text = item.itemName;
        itemPrice.text = item.buyPrice.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuyAction(playerCurrency, item));
    }
}
