using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;

public class VendorDisplay : MonoBehaviour, IGameService
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private Transform vendorSlotsParent;
    [SerializeField] private GameObject vendorSlotPrefab;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    public event Action OnDisplayStart;

    private SaleManager saleManager;
    private readonly List<GameObject> slots = new();

    public void Setup(CurrencySO playerCurrency, VendorSO vendorData, Action<CurrencySO, ItemSO> onBuyAction)
    {
        saleManager = ServiceLocator.Get<SaleManager>();

        saleManager.OnInvemtoryFull += UpdatePrompt;
        saleManager.OnInsufficientFunds += UpdatePrompt;

        if (vendorSlotsParent.childCount > 0)
        {
            slots.Clear();

            for (int i = 0; i < vendorSlotsParent.childCount; i++)
            {
                Destroy(vendorSlotsParent.GetChild(i).gameObject);
            }
        }

        for (int i = 0; i < vendorData.AvaiableItems.Count; i++)
        {
            var currentSlot = Instantiate(vendorSlotPrefab, vendorSlotsParent);
            slots.Add(currentSlot);

            ItemSO currentItem = vendorData.AvaiableItems[i];
            slots[i].GetComponent<VendorSlot>().Setup(playerCurrency, currentItem, onBuyAction);
        }

        SetWindowDisplay(true);
    }

    public void SetWindowDisplay(bool value)
    {
        if (value == false)
        {
            saleManager.OnInvemtoryFull -= UpdatePrompt;
            saleManager.OnInsufficientFunds -= UpdatePrompt;
            UpdatePrompt(false, string.Empty);
        }

        canvas.SetActive(value);
        OnDisplayStart?.Invoke();
    }

    private void UpdatePrompt(bool value,string prompt)
    {
        messageText.text = prompt;
        messagePanel.SetActive(value);
    }

    public GameObject GetFirstButton() => slots[0].GetComponent<VendorSlot>().buyButton.gameObject;
}
