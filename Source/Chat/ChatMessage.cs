using Godot;
using Godot.Collections;
using MessagePack;
namespace NullCyan.Sandboxnator.Chat;

[MessagePackObject]
public partial class ChatMessage
{
    [Key(0)]
    public string Content { get; set; }
    [Key(1)]
    public int PlayerId { get; set; }

    // Constructor
    public ChatMessage(string content, int playerId)
    {
        Content = content;
        PlayerId = playerId;
    }

}