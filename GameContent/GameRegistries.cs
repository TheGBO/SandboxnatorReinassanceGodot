public partial class GameRegistries : Singleton<GameRegistries>
{
    public Registry<ItemData> ItemRegistry { get; set; } = new();
}