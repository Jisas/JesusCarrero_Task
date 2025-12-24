using UnityEngine;

public class VendorInteractable : MonoBehaviour, IInteractable
{
    [Header("Requirements")]
    [SerializeField] private VendorSO vendorData;

    public InteractionType Type => InteractionType.Buy;
    public string InteractionPrompt => "Buy";

    public void Interact(PlayerController player)
    {
        var playerCurrency = ServiceLocator.Get<CurrencySO>();
        var saleManager = ServiceLocator.Get<SaleManager>();
        var vendorDisplay = ServiceLocator.Get<VendorDisplay>();

        if (vendorDisplay != null)
        {
            vendorDisplay.Setup(playerCurrency, vendorData, saleManager.Buy);
        }
    }
}