using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Sandboxnator.WorldAndScenes;

namespace NullCyan.Sandboxnator.Commands;

public class CommandGetPos : ChatCommand, IChatCommand
{
    public CommandGetPos()
    {
        Name = "getpos";
        Description = "get position of player by ID. Usage: !getpos <playerID>";
        Handler = Handle;
    }

    public void Handle(CommandContext ctx)
    {
        void SendPos(Player p)
        {
            string positionMessage =
            $"\n{p.ProfileData.PlayerName} is at [color=RED]X:[/color]{(int)p.GlobalPosition.X}, [color=GREEN]Y;[/color]{(int)p.GlobalPosition.Y}, [color=BLUE]Z:[/color]{(int)p.GlobalPosition.Z}; [color=orange]facing:[/color]{p.GlobalTransform.Basis.Z.Round()}";

            ctx.Reply(positionMessage);
        }

        if (ctx.Args.Length < 1)
        {
            SendPos(ctx.Sender);
            return;
        }

        if (int.TryParse(ctx.Args[0], out int idArg))
        {
            Player p = World.Instance.GetPlayerById(idArg);
            if (p == null)
            {
                ctx.ReplyErr($"could not find player of ID {idArg}");
                return;
            }
            SendPos(p);
        }
        else
        {
            ctx.ReplyErr("INVALID ARGUMENTS.");
        }
    }
}