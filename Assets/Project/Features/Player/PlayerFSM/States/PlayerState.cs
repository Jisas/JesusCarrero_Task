public abstract class PlayerState
{
    protected PlayerController context;

    public PlayerState(PlayerController context) => this.context = context;

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }
}