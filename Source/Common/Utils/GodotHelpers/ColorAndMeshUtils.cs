using Godot;
using System;
namespace NullCyan.Util.GodotHelpers;

class ColorAndMeshUtils
{
    public static Color InvertColor(Color color)
    {
        Color maxxedColor = new(1, 1, 1, 1);
        Color resultColor = maxxedColor - color;
        resultColor.A = color.A;
        return resultColor;
    }

    public static void ChangeMeshColor(MeshInstance3D model, Color color)
    {
        //Change the active model 0
        var currentMaterial = model.GetActiveMaterial(0);
        if (currentMaterial is StandardMaterial3D stdMat)
        {
            stdMat = (StandardMaterial3D)stdMat.Duplicate();
            stdMat.AlbedoColor = color;
            model.MaterialOverride = stdMat;
        }
    }

    /// <summary>
    /// Set the ability of a mesh to physically show even when inside another model.
    /// </summary>
    /// <param name="model"></param>
    /// <param name="clip"></param>
    public static void SetMeshClip(MeshInstance3D model, bool clip)
    {
        var handMaterial = model.GetActiveMaterial(0);
        if (handMaterial is StandardMaterial3D stdMat)
        {
            stdMat = (StandardMaterial3D)stdMat.Duplicate();
            stdMat.UseZClipScale = true;
            model.MaterialOverride = stdMat;
        }
    }
}

