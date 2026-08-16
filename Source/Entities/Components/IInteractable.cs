using Godot;
using Godot.Collections;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;
using System;

namespace NullGarel.Sandboxnator.Building;

public partial interface IInteractable
{

    /// <summary>
    /// RUNS ON SERVER.
    /// </summary>
    public abstract void Interact();

}
