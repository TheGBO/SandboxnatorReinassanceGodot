using Godot;

public abstract partial class AbstractComponent<T> : Node, IComponent where T : Node
{
    public T ComponentParent { get; private set; }

    public void Initialize(ComponentHolder holder)
    {
        // Try to cast holder's parent to the expected type
        ComponentParent = holder.GetParent<T>();

        if (ComponentParent == null)
        {
            GD.PrintErr($"{GetType().Name} expected a parent of type {typeof(T).Name}, but got {holder.GetParent().GetType().Name}.");
        }

        OnInitialized();
    }

    // Optional: override this instead of Initialize() directly
    protected virtual void OnInitialized() {}
}
