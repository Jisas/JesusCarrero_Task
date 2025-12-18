using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController context) : base(context) { }

    public override void Enter() 
    {
        if (context == null) return;
        if (context.DebugMode) Debug.Log("Enter Idle State");
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
        if (context == null) return;
        if (context.DebugMode) Debug.Log("Exit Idle State");
    }
}