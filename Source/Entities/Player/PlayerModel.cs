using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Entity.PlayerCosmetics;
using NullCyan.Util.GodotHelpers;
using System;
namespace NullCyan.Sandboxnator.Entity;

/// <summary>
/// A class that holds shared info about the player model,
/// used both by the player instance in the world and also 
/// by the new profile editor
/// </summary>
public partial class PlayerModel : Node3D
{
    [ExportGroup("Model/cosmetic")]
    [Export] public Array<MeshInstance3D> modelsToColor;
    [Export] public MeshInstance3D handMesh;
    [Export] public MeshInstance3D faceMesh;


    public void UpdateVisual(PlayerProfileData profile)
    {
        foreach (MeshInstance3D element in modelsToColor)
        {
            if (IsInstanceValid(element))
                ColorAndMeshUtils.ChangeMeshColor(element, profile.PlayerColor);
        }
        ColorAndMeshUtils.ChangeMeshTexture(faceMesh, PlayerFaceRegistryManager.GetTextureByFaceId(profile.PlayerFaceId));
    }
}
