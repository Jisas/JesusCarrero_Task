using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController context) : base(context) { }

    public override void Enter() 
    {
        if (context.DebugMode) Debug.Log("Enter Idle State");
        context.InputReader.Input.InteractEvent += OnInteractPressed;
    }

    public override void Update() 
    {
        Vector2 direction = context.InputReader.Input.MoveValue;

        if (direction != Vector2.zero)
        {
            context.TransitionTo(context.Move);
        }
    }

    public override void Exit() 
    {
        if (context.DebugMode) Debug.Log("Exit Idle State");
        context.InputReader.Input.InteractEvent -= OnInteractPressed;
    }

    private void OnInteractPressed()
    {
        if (context.Interactor.HasTarget)
        {
            context.TransitionTo(context.Interact);
        }
    }
}