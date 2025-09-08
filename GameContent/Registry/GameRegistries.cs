using Godot;
using NullCyan.Sandboxnator.Commands;
using NullCyan.Sandboxnator.Item;
using NullCyan.Util;
namespace NullCyan.Sandboxnator.Registry;

public partial class GameRegistries : Singleton<GameRegistries>
{
    public Registry<ItemData> ItemRegistry { get; set; } = new();
    public Registry<ChatCommand> CommandRegistry { get; set; } = new();
    public Registry<PackedScene> BuildingRegistry { get; set; } = new();

    public override void _Ready()
    {
        InitializeRegistries();
    }

    

    private void InitializeRegistries()
    {
        ItemRegistryManager itemRegistryManager = new();
        itemRegistryManager.Register();
        CommandRegistryManager commandRegistryManager = new();
        commandRegistryManager.Register();
    }
}