using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        if (context.DebugMode) Debug.Log("Enter Move State");
        context.InputReader.Input.InteractEvent += OnInteractPressed;
    }

    public override void Update()
    {
        Vector2 direction = context.InputReader.Input.MoveValue;

        if (direction == Vector2.zero)
        {
            context.TransitionTo(context.Idle);
            return;
        }

        context.Mover.Move(direction, direction.magnitude);
    }

    public override void Exit()
    {
        if (context.DebugMode) Debug.Log("Exit Move State");
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