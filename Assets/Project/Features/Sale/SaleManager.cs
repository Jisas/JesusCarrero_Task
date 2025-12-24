using UnityEngine;
using System;

public class SaleManager : MonoBehaviour, IGameService
{
    public event Action OnSaleEnded;
    public event Action<bool, string> OnInvemtoryFull;
    public event Action<bool, string> OnInsufficientFunds;

    public void OnEndSale() => OnSaleEnded?.Invoke();

    private const string inventoryFullPrompt = "Invemtory Full";
    private const string InsufficientFundsPrompt = "Insufficient Funds";

    public void Buy(CurrencySO playerCurrency, ItemSO item)
    {
        string prompt = string.Empty;

        if (playerCurrency.TotalGold >= item.buyPrice)
        {
            // 2. Is there space in the inventory?
            var inventory = ServiceLocator.Get<InventoryManager>().Inventory;

            if (inventory.HasSpace(item))
            {
                if (playerCurrency.TrySpend(item.buyPrice))
                {
                    inventory.AddItem(item, 1);
                    Debug.Log($"Buyed {item.itemName}");
                }
            }
            else
            {
                prompt = inventoryFullPrompt;
                OnInvemtoryFull?.Invoke(true, prompt);
                Debug.Log("<color=red>SInventario full.</color>");
            }
        }
        else
        {
            prompt = InsufficientFundsPrompt;
            OnInsufficientFunds?.Invoke(true, prompt);
            Debug.Log("<color=red>You don't have enough money.</color>");
        }
    }
}
