namespace NullGarel.Sandboxnator.Chat;


public class ChatMessage
{
    public string Content { get; }
    public int PlayerId { get; }

    public ChatMessage(string content, int playerId)
    {
        Content = content;
        PlayerId = playerId;
    }
}