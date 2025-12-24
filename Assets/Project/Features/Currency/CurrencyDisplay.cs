using UnityEngine;
using TMPro;

public class CurrencyDisplay : MonoBehaviour, IInitializable
{
    [SerializeField] private TextMeshProUGUI textAmount;
    private CurrencySO currencyData;

    public void Initialize()
    {
        currencyData = ServiceLocator.Get<CurrencySO>();
        currencyData.OnCurrencyUpdated += UpdateUI;
    }

    public void UpdateUI(int amount)
    {
        textAmount.text = amount.ToString();
    }

    private void OnDestroy()
    {
        currencyData.OnCurrencyUpdated -= UpdateUI;
    }

}