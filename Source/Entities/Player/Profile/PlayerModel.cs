using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Entity.PlayerCosmetics;
using NullGarel.Util.GodotHelpers;
namespace NullGarel.Sandboxnator.Entity;

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
            if (!IsInstanceValid(element)) continue;
            element.ChangeMeshColor(profile.PlayerColor);
        }
        faceMesh.ChangeMeshTexture(PlayerFaceRegistryManager.GetTextureByFaceId(profile.PlayerFaceId));
    }
}
