using UnityEngine;

public class PlayerController : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private PlayerVisuals visuals;

    public EntityInputReader InputReader { get; private set; }
    public PlayerMover Mover { get; private set; }
    public PlayerInteractor Interactor { get; set; }
    public PlayerVisuals Visuals { get; private set; }

    private PlayerState currentState;

    // Status cache to avoid constant 'new'
    public IdleState Idle { get; private set; }
    public MoveState Move { get; private set; }
    public InteractState Interact { get; private set; }
    public InventoryState Inventory { get; private set; }

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

        // Events subscription
        InputReader.Input.InventoryRequest += HandleInventoryInput;
    }

    private void OnEnable()
    {
        //Mover.OnSpeedChanged += visuals.UpdateMoveAnimation;
    }

    private void Start() => TransitionTo(Idle);

    private void Update() => currentState?.Update();

    private void OnDisable()
    {
        //Mover.OnSpeedChanged -= visuals.UpdateMoveAnimation;
    }

    public void TransitionTo(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void HandleInventoryInput()
    {
        // 1. Si ya estamos en el inventario, no hacemos nada (el estado se cerrará a sí mismo)
        if (currentState is InventoryState) return;

        // 2. Si estamos en un estado que NO permite abrir inventario (ej: Muerto, Cinemática), salimos
        // if (stateMachine.CurrentState is DeathState) return;

        // 3. Si todo está bien, forzamos la transición
        TransitionTo(Inventory);
    }
}