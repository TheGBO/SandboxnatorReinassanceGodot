using NullCyan.Sandboxnator.Chat;
using NullCyan.Sandboxnator.Entity;
namespace NullCyan.Sandboxnator.Commands;

public class CommandContext
{
    public string RawInput { get; }
    public string CommandName { get; }
    public string[] Args { get; }
    public Player Sender;
    public int SenderId => Sender?.componentHolder.entityId ?? -1;

    public CommandContext(string rawInput, string commandName, string[] args, Player sender)
    {
        RawInput = rawInput;
        CommandName = commandName;
        Args = args;
        Sender = sender;
    }

    public void Reply(string msg)
    {
        ChatManager.Instance.SendPlayerlessMessage(msg, SenderId);
    }

    public void ReplyErr(string msg)
    {
        Reply($"[color=red](ERROR):[/color]{msg}");
    }

}