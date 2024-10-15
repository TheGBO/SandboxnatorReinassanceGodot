using Godot;
using Godot.Collections;

public partial class ToolUsageArgs
{
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public int PlayerId { get; set; }

    // Constructor
    public ToolUsageArgs(Vector3 position, Vector3 normal, int playerId)
    {
        Position = position;
        Normal = normal;
        PlayerId = playerId;
    }

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "position", Position },
            { "normal", Normal },
            { "playerId", PlayerId }
        };
    }

    // Static method to create an instance from a Dictionary
    public static ToolUsageArgs FromDictionary(Dictionary data)
    {
        return new ToolUsageArgs(
            (Vector3)data["position"],
            (Vector3)data["normal"],
            (int)data["playerId"]
        );
    }
}