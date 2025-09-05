using Godot;
namespace NullCyan.Util.ComponentSystem;

/// <summary>
/// Acts as a representation of an entity, it should be a direct child of a node that is to be considered an entity in this
/// custom entity system.
/// </summary>
public partial class ComponentHolder : Node
{
    public int entityId;

    public override void _EnterTree()
    {
        foreach (Node child in GetChildren())
        {
            if (child is IComponent component)
            {
                component.Initialize(this);
            }
        }
    }

    // Utility for typed components to grab other components
    public T GetComponent<T>() where T : class, IComponent
    {
        foreach (Node child in GetChildren())
            if (child is T match)
                return match;
        return null;
    }
}
