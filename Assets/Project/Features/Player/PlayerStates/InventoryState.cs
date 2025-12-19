
public class InventoryState : PlayerState
{
    public InventoryState(PlayerController context) : base(context) { }

    public override void Enter()
    {
        // 1. Cambiamos al mapa de UI (esto ya bloquea el movimiento automáticamente)
        context.InputReader.SwitchToUI();

        // 2. Notificamos a la UI que se muestre (vía evento o referencia directa)
        // Usaremos un evento para mantener el desacoplamiento
        InventoryEvents.OnInventoryRequest?.Invoke(true);

        // 3. Suscribimos el evento para cerrar
        context.InputReader.Input.InventoryRequest += ToggleInventory;
    }

    private void ToggleInventory()
    {
        // Al presionar la misma tecla, volvemos al estado anterior (Idle/Move)
        context.TransitionTo(context.Idle);
    }

    public override void Exit()
    {
        context.InputReader.Input.InventoryRequest -= ToggleInventory;
        context.InputReader.SwitchToPlayer();
        InventoryEvents.OnInventoryRequest?.Invoke(false);
    }
}