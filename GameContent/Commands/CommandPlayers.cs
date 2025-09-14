using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Sandboxnator.WorldAndScenes;

namespace NullCyan.Sandboxnator.Commands;

public class CommandPlayers : ChatCommand, IChatCommand
{
    public CommandPlayers()
    {
        Name = "players";
        Description = "list players and their id's";
        Handler = Handle;
    }

    public void Handle(CommandContext ctx)
    {
        string playerListMsg = "Connected players in to the server:";
        foreach (Player p in World.Instance.GetPlayers())
        {
            playerListMsg += $"\n[Player ID:{p.componentHolder.entityId}] : [color={p.ProfileData.PlayerColor.ToHtml()}]{p.ProfileData.PlayerName}[/color]";
        }
        ctx.Reply(playerListMsg);
    }
}