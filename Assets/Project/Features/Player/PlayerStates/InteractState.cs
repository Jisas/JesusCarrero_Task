using UnityEngine;

public class InteractState : PlayerState
{
    public InteractState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        if (context.DebugMode) Debug.Log("Enter Interact State");
        context.Interactor.DoInteraction(context);
        context.TransitionTo(context.Idle);
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {
        if (context.DebugMode) Debug.Log("Exit Interact State");
    }
}