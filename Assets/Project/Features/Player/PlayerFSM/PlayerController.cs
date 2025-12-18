using UnityEngine;

public class PlayerController : MonoBehaviour, IInitializable, IGameService
{
    [SerializeField] private PlayerVisuals Visuals;
    [SerializeField] private bool debugMode;

    public PlayerMover Mover { get; private set; }
    public EntityInputReader InputReader { get; private set; }
    public bool DebugMode => debugMode;

    private PlayerState currentState;

    // Status cache to avoid constant 'new'
    public IdleState Idle { get; private set; }
    public MoveState Move { get; private set; }

    public void Initialize()
    {
        Mover = GetComponent<PlayerMover>();
        InputReader = GetComponent<EntityInputReader>();

        // Initialize states only once
        Idle = new IdleState(this);
        Move = new MoveState(this);
    }

    private void OnEnable()
    {
        Mover.OnSpeedChanged += Visuals.UpdateMoveAnimation;
    }

    private void Start() => TransitionTo(Idle);

    private void Update() => currentState?.Update();

    private void OnDisable()
    {
        Mover.OnSpeedChanged -= Visuals.UpdateMoveAnimation;
    }

    public void TransitionTo(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
}