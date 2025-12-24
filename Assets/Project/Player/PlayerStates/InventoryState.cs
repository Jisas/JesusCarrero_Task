
public class InventoryState : PlayerState
{
    public InventoryState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        context.InputReader.Input.InventoryRequest += ToggleInventory;
        ProjectExtensions.OnInventoryRequest?.Invoke(true);
        context.InputReader.SwitchToUI();
    }

    private void ToggleInventory()
    {
        context.TransitionTo(context.Idle);
    }

    public override void Exit()
    {
        context.InputReader.Input.InventoryRequest -= ToggleInventory;
        ProjectExtensions.OnInventoryRequest?.Invoke(false);
        context.InputReader.SwitchToPlayer();
    }
}