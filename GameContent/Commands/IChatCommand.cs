namespace NullCyan.Sandboxnator.Commands;

public interface IChatCommand
{
    void Handle(CommandContext ctx);
}