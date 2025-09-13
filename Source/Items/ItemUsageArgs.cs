using Godot;
using Godot.Collections;
using MessagePack;
namespace NullCyan.Sandboxnator.Item;

/// <summary>
/// A Data Transfer Object (DTO) regarding the usage of items from players.
/// </summary>
/// 
[MessagePackObject]
public partial class ItemUsageArgs
{
    [Key(0)]
    public Vector3 Position { get; set; }
    [Key(1)]
    public Vector3 Normal { get; set; }
    [Key(2)]
    public int PlayerId { get; set; }

    // Constructor
    public ItemUsageArgs(Vector3 position, Vector3 normal, int playerId)
    {
        Position = position;
        Normal = normal;
        PlayerId = playerId;
    }

    public override string ToString()
        => $"Item usage args | Pos={Position}, Normal={Normal}, PlayerId={PlayerId}";
}