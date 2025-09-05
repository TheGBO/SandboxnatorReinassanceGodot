using System.Linq;
using Godot;
using NullCyan.Sandboxnator.Chat;
using NullCyan.Sandboxnator.Entity;
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
                    playerListMsg += $"\n[Player ID:{p.componentHolder.entityId}] : [color={p.profileData.PlayerColor.ToHtml()}]{p.profileData.PlayerName}[/color]";
                }
                ctx.Reply(playerListMsg);
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
        string name = split[0].ToLower();
        string[] args = split.Length > 1 ? split[1..] : [];

        ChatCommand cmd = GameRegistries.Instance.CommandRegistry.Get(name);
        if (cmd != null)
        {
            var context = new CommandContext(rawInput, name, args, sender);
            cmd.Execute(context);
            return true;
        }
        else
        {
            ChatManager.Instance.SendPlayerlessMessage($"Unknown command: {name}", sender.componentHolder.entityId);
            return false;
        }


    }
}