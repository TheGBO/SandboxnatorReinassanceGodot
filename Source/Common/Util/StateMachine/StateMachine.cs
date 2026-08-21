namespace NullGarel.Util.StateMachine;

//TODO: document this too
public class StateMachine<TContext>
{
    public IState<TContext> CurrentState { get; private set; }
    private readonly TContext _context;

    public StateMachine(TContext context, IState<TContext> initialState)
    {
        _context = context;
        ChangeState(initialState);
    }

    public void PhysicsProcess(double delta)
    {
        var next = CurrentState.CheckTransitions(_context);
        if (next != null)
            ChangeState(next);

        CurrentState.PhysicsProcess(_context, delta);
    }

    public void ChangeState(IState<TContext> newState)
    {
        CurrentState?.Exit(_context);
        CurrentState = newState;
        CurrentState.Enter(_context);
    }
}