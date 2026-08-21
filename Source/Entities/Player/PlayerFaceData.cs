using Godot;
using System;
namespace NullGarel.Sandboxnator.Entity.PlayerCosmetics;

//TODO: Make PlayerFaceData and ItemData inherit a common resource called "SandboxnatorAssetData"
[GlobalClass]
public partial class PlayerFaceData : Resource
{
    [ExportGroup("Basic properties")]
    [Export] public string PlayerFaceId { get; private set; }
    [Export] public Texture2D FaceTexture { get; private set; }

}
