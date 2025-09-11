using System.Linq;
using Godot;
using NullCyan.Sandboxnator.Chat;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Sandboxnator.Network;
using NullCyan.Sandboxnator.Registry;
using NullCyan.Sandboxnator.WorldAndScenes;
using NullCyan.Util;

namespace NullCyan.Sandboxnator.Commands;

public class CommandRegistryManager : IRegistryManager
{
    /// <summary>
    /// Register game chat commands
    /// </summary>
    public void Register()
    {
        RegisterCommand(
            new ChatCommand()
            .WithName("help")
            .WithDescription("displays all commands registered in the game.")
            .WithHandler((ctx) =>
            {
                string helpListMsg = "[color=cyan]Chat commands registered in the game[/color]:";
                foreach (ChatCommand cmd in GameRegistries.Instance.CommandRegistry.GetAllValues())
                {
                    helpListMsg += $"\n [color=orange][{cmd.Name}][/color]:{cmd.Description}";
                }
                ctx.Reply(helpListMsg);
            })
        );

        RegisterCommand(new ChatCommand()
            .WithName("players")
            .WithDescription("Retrieves a list of all players connected and their respective numeric identifiers")
            .WithHandler((ctx) =>
            {
                string playerListMsg = "Connected players in to the server:";
                foreach (Player p in World.Instance.GetPlayers())
                {
                    playerListMsg += $"\n[Player ID:{p.componentHolder.entityId}] : [color={p.ProfileData.PlayerColor.ToHtml()}]{p.ProfileData.PlayerName}[/color]";
                }
                ctx.Reply(playerListMsg);
            })
        );

        RegisterCommand(new ChatCommand()
            .WithName("getpos")
            .WithDescription("retrieves the position of a player by their ID in XYZ coordinates. Usage: !getpos <playerID>")
            .WithHandler((ctx) =>
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
            })
        );
    }


    public static void RegisterCommand(ChatCommand command)
    {
        if (GameRegistries.Instance.CommandRegistry.Contains(command.Name))
        {
            GD.PrintErr($"[Commands] Command '{command.Name}' already exists!");
            return;
        }

        GameRegistries.Instance.CommandRegistry.Register(command.Name.ToLower(), command);
        GD.Print($"[Commands] Registered '{command.Name}'");
    }

    public static bool ExecuteCommand(Player sender, string rawInput)
    {

        if (!rawInput.StartsWith("!"))
            return false;

        string[] split = rawInput.Substring(1).Split(' ');
        string commandKeyName = split[0].ToLower();
        string[] args = split.Length > 1 ? split[1..] : [];

        try
        {
            ChatCommand cmd = GameRegistries.Instance.CommandRegistry.Get(commandKeyName);
            if (cmd != null)
            {
                var context = new CommandContext(rawInput, commandKeyName, args, sender);
                cmd.Execute(context);
                return true;
            }
            else
            {
                return SendInvalidCommandWarning(commandKeyName, sender);
            }
        }
        catch (System.Exception)
        {
            return SendInvalidCommandWarning(commandKeyName, sender);
        }


    }

    private static bool SendInvalidCommandWarning(string commandKeyName, Player sender)
    {
        ChatManager.Instance.SendPlayerlessMessage($"Unknown command or invalid arguments when attempting to execute: {commandKeyName}, try typing \"!help\" in order to see available in-game commands and their respective usage instructions.", sender.componentHolder.entityId);
        //Seems counterintuitive, but returnig true means that there was an attempt, not necessarily success.
        return true;
    }
}