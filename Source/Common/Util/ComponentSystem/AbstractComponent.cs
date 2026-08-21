using Godot;
using NullGarel.Util.Log;
namespace NullGarel.Util.ComponentSystem;

[GodotClassName(nameof(AbstractComponent<T>))]
[Icon("res://Assets/Textures/Components/component.png")]
public abstract partial class AbstractComponent<T> : Node, IComponent where T : Node
{
    public T ComponentParent { get; private set; }

    private ComponentHolder _holder;
    public ComponentHolder Holder => _holder;

    public void Initialize(ComponentHolder holder)
    {
        // Try to cast holder's parent to the expected type
        ComponentParent = holder.GetParent<T>();

        if (ComponentParent == null)
        {
            NcLogger.Log($"{GetType().Name} expected a parent of type {typeof(T).Name}, but got {holder.GetParent().GetType().Name}.");
        }

        _holder = holder;

        OnInitialized();
    }

    /// <summary>
    /// An alias to avoid the littering of ComponentParent.componentHolder.GetComponent\<T\>();
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    /// <returns></returns>
    public TC GetComponent<TC>() where TC : class, IComponent
    {
        return _holder.GetComponent<TC>();
    }

    // Optional: override this instead of Initialize() directly
    protected virtual void OnInitialized() { }
}
