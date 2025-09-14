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
        RegisterCommand(new CommandHelp());
        RegisterCommand(new CommandPlayers());
        RegisterCommand(new CommandGetPos());
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