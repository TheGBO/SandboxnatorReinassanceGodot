using Godot;
using System;
namespace NullCyan.Util;

class ColorUtils
{
    public static Color InvertColor(Color color)
    {
        Color maxxedColor = new Color(1, 1, 1, 1);
        Color resultColor = maxxedColor - color;
        resultColor.A = color.A;
        return resultColor;
    }

    public static void ChangeMeshColor(MeshInstance3D model, Color color)
    {
        var currentMaterial = model.GetActiveMaterial(0);
		if (currentMaterial is StandardMaterial3D stdMat)
		{
			stdMat = (StandardMaterial3D)stdMat.Duplicate();
			stdMat.AlbedoColor = color;
			model.MaterialOverride = stdMat;
		}
    }
}

