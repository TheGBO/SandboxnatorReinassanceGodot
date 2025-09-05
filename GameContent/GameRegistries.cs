using NullCyan.Sandboxnator.Item;
using NullCyan.Util;
namespace NullCyan.Sandboxnator.Registry;

public partial class GameRegistries : Singleton<GameRegistries>
{
    public Registry<ItemData> ItemRegistry { get; set; } = new();
}