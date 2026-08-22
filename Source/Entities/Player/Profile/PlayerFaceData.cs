using Godot;
namespace NullGarel.Sandboxnator.Entity.PlayerCosmetics;

[GlobalClass]
public partial class PlayerFaceData : Resource
{
    [ExportGroup("Basic properties")]
    [Export] public string PlayerFaceId { get; private set; }
    [Export] public Texture2D FaceTexture { get; private set; }

}
