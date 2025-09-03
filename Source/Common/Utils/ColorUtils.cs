using Godot;
using System;
namespace GBOUtils;

class ColorUtils
{
    public static Color InvertColor(Color color)
    {
        Color maxxedColor = new Color(1, 1, 1, 1);
        Color resultColor = maxxedColor - color;
        resultColor.A = color.A;
        return resultColor;
    }
}

