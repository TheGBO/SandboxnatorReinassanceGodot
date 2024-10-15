using Godot;
using System;

public partial class AbstractPlayerComponent : Node3D
{
    [Export] public Player parent;

    /// <summary>
    /// Returns the id of the player
    /// </summary>
    public int GetPlayerId()
    {
        return parent.playerId;
    }
}