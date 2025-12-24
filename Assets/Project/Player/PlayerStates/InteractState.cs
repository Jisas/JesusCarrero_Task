using UnityEngine;
using UnityEngine.EventSystems;

public class InteractState : PlayerState
{
    private IInteractable target;
    private readonly DialogueManager _dialogueManager;
    private readonly SaleManager _saleManager;
    private readonly VendorDisplay _vendorDisplay;

    public InteractState(PlayerController context) : base(context)
    {
        _dialogueManager = ServiceLocator.Get<DialogueManager>();
        _saleManager = ServiceLocator.Get<SaleManager>();
        _vendorDisplay = ServiceLocator.Get<VendorDisplay>();
    }

    public override void Enter()
    {
        target = context.Interactor.CurrentTarget;

        switch (target.Type)
        {
            case InteractionType.Pickup:
                OnItemInteract();
                break;

            case InteractionType.Talk:
                OnNPCInteract();
                break;

            case InteractionType.Buy:
                OnBuyInteract();
                break;
        }
    }

    private void OnItemInteract()
    {
        context.Interactor.DoInteraction(context);
        context.TransitionTo(context.Idle);
    }

    private void OnNPCInteract()
    {
        context.InputReader.Input.InteractEvent += OnInteractPressed;
        _dialogueManager.OnDialogueEnded += EndDialogue;
        context.Interactor.DoInteraction(context);
    }


    private void OnInteractPressed()
    {
        if (_dialogueManager.IsDialogueActive)
        {
            _dialogueManager.AdvanceDialogue();
        }
    }

    private void EndDialogue()
    {
        context.TransitionTo(context.Idle);
    }

    private void OnBuyInteract()
    {
        _saleManager.OnSaleEnded += EndSale;
        _vendorDisplay.OnDisplayStart += OnStartSale;
        context.Interactor.DoInteraction(context);
    }

    private void OnStartSale()
    {
        context.InputReader.SwitchToUI();
        EventSystem.current.SetSelectedGameObject(_vendorDisplay.GetFirstButton());
    }

    private void EndSale()
    {
        _vendorDisplay.SetWindowDisplay(false);
        context.TransitionTo(context.Idle);
    }

    public override void Exit()
    {
        switch (target.Type)
        {
            case InteractionType.Pickup:
                break;

            case InteractionType.Talk:
                context.InputReader.Input.InteractEvent -= OnInteractPressed;
                _dialogueManager.OnDialogueEnded -= EndDialogue;
                break;

            case InteractionType.Buy:
                _vendorDisplay.OnDisplayStart -= OnStartSale;
                _saleManager.OnSaleEnded -= EndSale;
                context.InputReader.SwitchToPlayer();
                break;
        }
    }

}