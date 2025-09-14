using NullCyan.Sandboxnator.Registry;

namespace NullCyan.Sandboxnator.Commands;

public class CommandHelp : ChatCommand, IChatCommand
{
    public CommandHelp()
    {
        Name = "help";
        Description = "displays all commands registered in the game.";
        Handler = Handle;
    }

    public void Handle(CommandContext ctx)
    {
        string helpListMsg = "[color=cyan]Chat commands registered in the game[/color]:";
        foreach (ChatCommand cmd in GameRegistries.Instance.CommandRegistry.GetAllValues())
        {
            helpListMsg += $"\n [color=orange][{cmd.Name}][/color]:{cmd.Description}";
        }
        ctx.Reply(helpListMsg);
    }
}