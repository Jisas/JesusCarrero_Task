using UnityEngine;

public class MenuState : PlayerState
{
    public MenuState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        context.InputReader.Input.MenuRequest += ToggleMenu;
        ProjectExtensions.OnMenuRequest?.Invoke(true);
        context.InputReader.SwitchToUI();
    }

    private void ToggleMenu()
    {
        context.TransitionTo(context.Idle);
    }

    public override void Exit()
    {
        context.InputReader.Input.MenuRequest -= ToggleMenu;
        ProjectExtensions.OnMenuRequest?.Invoke(false);
        context.InputReader.SwitchToPlayer();
    }
}
