using UnityEngine;

public class InteractState : PlayerState
{
    private IInteractable target;
    private readonly DialogueManager _dialogueManager;

    public InteractState(PlayerController context) : base(context)
    {
        _dialogueManager = ServiceLocator.Get<DialogueManager>();
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
        }
    }

    private void OnItemInteract()
    {
        //context.Visuals.PlayAnimation("Pickup");
        context.Interactor.DoInteraction(context);
        context.TransitionTo(context.Idle);
    }

    private void OnNPCInteract()
    {
       //context.Visuals.PlayAnimation("Talk");
        context.InputReader.Input.InteractEvent += OnInteractPressed;
        _dialogueManager.OnDialogueEnded += EndDialogue;

        // Execute the initial interaction (which triggers the dialogue in the NPC).
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

    public override void Exit()
    {
        if (target.Type == InteractionType.Talk)
        {
            context.InputReader.Input.InteractEvent -= OnInteractPressed;
            _dialogueManager.OnDialogueEnded -= EndDialogue;
        }
    }

}