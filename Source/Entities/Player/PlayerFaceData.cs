using Godot;
using System;
namespace NullCyan.Sandboxnator.Entity.PlayerCosmetics;

//TODO: Make PlayerFaceData and ItemData inherit a common resource called "SandboxnatorAssetData"
[GlobalClass]
public partial class PlayerFaceData : Resource
{
    [ExportGroup("Basic properties")]
    [Export] public string playerFaceId;
    [Export] public Texture2D faceTexture;

}
