using Godot;
using Godot.Collections;
namespace NullCyan.Sandboxnator.Chat;

public partial class ChatMessage
{
    public string Content { get; set; }
    public int PlayerId { get; set; }

    // Constructor
    public ChatMessage(string content, int playerId)
    {
        Content = content;
        PlayerId = playerId;
    }

    public Dictionary ToDictionary()
    {
        return new Dictionary
        {
            { "content", Content },
            { "playerId", PlayerId }
        };
    }

    // Static method to create an instance from a Dictionary
    public static ChatMessage FromDictionary(Dictionary data)
    {
        return new ChatMessage(
            (string)data["content"],
            (int)data["playerId"]
        );
    }
}