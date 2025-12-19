
public class InventoryState : PlayerState
{
    public InventoryState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        context.InputReader.SwitchToUI();
        InventoryEvents.OnInventoryRequest?.Invoke(true);
        context.InputReader.Input.InventoryRequest += ToggleInventory;
    }

    private void ToggleInventory()
    {
        context.TransitionTo(context.Idle);
    }

    public override void Exit()
    {
        context.InputReader.Input.InventoryRequest -= ToggleInventory;
        context.InputReader.SwitchToPlayer();
        InventoryEvents.OnInventoryRequest?.Invoke(false);
    }
}