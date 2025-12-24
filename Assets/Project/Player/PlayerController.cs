using UnityEngine;

public class PlayerController : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private PlayerVisuals visuals;

    public EntityInputReader InputReader { get; private set; }
    public PlayerMover Mover { get; private set; }
    public PlayerInteractor Interactor { get; set; }
    public PlayerVisuals Visuals => visuals;

    private PlayerState currentState;

    // Status cache to avoid constant 'new'
    public IdleState Idle { get; private set; }
    public MoveState Move { get; private set; }
    public InteractState Interact { get; private set; }
    public InventoryState Inventory { get; private set; }
    public MenuState Menu { get; private set; }
    public DrinkState Drink { get; private set; }

    public void Initialize()
    {
        Mover = GetComponent<PlayerMover>();
        Interactor = GetComponent<PlayerInteractor>();
        InputReader = GetComponent<EntityInputReader>();

        // Initialize states only once
        Idle = new IdleState(this);
        Move = new MoveState(this);
        Interact = new InteractState(this);
        Inventory = new InventoryState(this);
        Menu = new MenuState(this);
        Drink = new DrinkState(this);

        // Events subscription
        InputReader.Input.InventoryRequest += HandleInventoryInput;
        InputReader.Input.MenuRequest += HandleMenuInput;
        Mover.OnSpeedChanged += visuals.UpdateMoveAnimation;
        Mover.OnGroundedChanged += visuals.UpdateGroundedStatus;
    }

    public void TransitionTo(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void HandleInventoryInput()
    {
        if (currentState is InventoryState) return;
        if (currentState is MenuState) return;
        if (currentState is InteractState) return;
        TransitionTo(Inventory);
    }

    private void HandleMenuInput()
    {
        if (currentState is MenuState) return;
        if (currentState is InventoryState) return;
        if (currentState is InteractState) return;
        TransitionTo(Menu);
    }

    private void Start() => TransitionTo(Idle);

    private void Update() => currentState?.Update();

    private void OnDestroy()
    {
        InputReader.Input.InventoryRequest -= HandleInventoryInput;
        InputReader.Input.MenuRequest -= HandleMenuInput;
        Mover.OnSpeedChanged -= visuals.UpdateMoveAnimation;
        Mover.OnGroundedChanged -= visuals.UpdateGroundedStatus;
    }
}