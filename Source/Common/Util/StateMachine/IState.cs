namespace NullGarel.Util.StateMachine;

public interface IState<TContext>
{
    /// <summary>
    /// called when the state begins
    /// </summary>
    /// <param name="context">context data type</param>
    void Enter(TContext context) { }

    /// <summary>
    /// called when the state ends
    /// </summary>
    /// <param name="context">context data type</param>
    void Exit(TContext context) { }

    /// <summary>
    /// To be called by godot's _PhysicsProcess(delta)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="delta"></param>
    void PhysicsProcess(TContext context, double delta) { }

    /// <summary>
    /// a loop that runs to evaluate conditions and possibly change state basded on said conditions
    /// returns null by default, to switch to a state, return a new <see cref="IState"/>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    IState<TContext> CheckTransitions(TContext context) => null;
}