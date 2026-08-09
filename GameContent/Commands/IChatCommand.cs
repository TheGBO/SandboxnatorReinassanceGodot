namespace NullGarel.Sandboxnator.Commands;

public interface IChatCommand
{
    void Handle(CommandContext ctx);
}