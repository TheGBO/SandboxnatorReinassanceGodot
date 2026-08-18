using Godot;
using Godot.Collections;
using System;
namespace NullGarel.Util.GodotHelpers;

public static class ColorAndMeshUtils
{
	public static Array<Color> PixelsOfImage(this Image img)
	{
		Array<Color> result = [];

		if (img.IsCompressed()) img.Decompress();

		Vector2I dimensions = new(img.GetWidth(), img.GetHeight());
		for (int y = 0; y < dimensions.Y; y++)
		{
			for (int x = 0; x < dimensions.X; x++)
			{
				Color pixelAt = img.GetPixel(x, y);
				result.Add(pixelAt);
			}
		}

		return result;
	}

	public static Color InvertColor(this Color color)
	{
		Color maxxedColor = new(1, 1, 1, 1);
		Color resultColor = maxxedColor - color;
		resultColor.A = color.A;
		return resultColor;
	}

	public static void ChangeMeshColor(this MeshInstance3D model, Color color)
	{
		if (model.Mesh == null) return;

		int surfaceCount = model.Mesh.GetSurfaceCount();

		for (int i = 0; i < surfaceCount; i++)
		{
			var currentMaterial = model.GetActiveMaterial(i);

			if (currentMaterial is StandardMaterial3D stdMat)
			{

				var newMat = (StandardMaterial3D)stdMat.Duplicate();
				newMat.AlbedoColor = color;


				model.SetSurfaceOverrideMaterial(i, newMat);
			}
		}
	}
	public static void ChangeMeshTexture(this MeshInstance3D model, Texture2D texture)
	{
		//Change the active model 0
		var currentMaterial = model.GetActiveMaterial(0);
		if (currentMaterial is StandardMaterial3D stdMat)
		{
			stdMat = (StandardMaterial3D)stdMat.Duplicate();
			stdMat.AlbedoTexture = texture;
			model.MaterialOverride = stdMat;
		}
	}

	/// <summary>
	/// Set the ability of a mesh to physically show even when inside another model.
	/// </summary>
	/// <param name="model"></param>
	/// <param name="clip"></param>
	public static void SetMeshClip(this MeshInstance3D model, bool clip = true)
	{
		var handMaterial = model.GetActiveMaterial(0);
		if (handMaterial is StandardMaterial3D stdMat)
		{
			stdMat = (StandardMaterial3D)stdMat.Duplicate();
			stdMat.UseZClipScale = clip;
			model.MaterialOverride = stdMat;
		}
	}
}
