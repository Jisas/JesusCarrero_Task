using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Currency", menuName = "Project/Currency")]
public class CurrencySO : ScriptableObject, IGameService
{
    [SerializeField] private int gold;
    public event Action<int> OnCurrencyUpdated;

    public int TotalGold => gold;

    public void AddGold(int amount)
    {
        gold += amount;
        OnCurrencyUpdated?.Invoke(gold);
    }

    public bool TrySpend(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            OnCurrencyUpdated?.Invoke(gold);
            return true;
        }
        return false;
    }

    public void SetGold(int amount)
    {
        gold = amount;
        OnCurrencyUpdated?.Invoke(gold);
    }
}