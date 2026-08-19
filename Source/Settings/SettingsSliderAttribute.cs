
using System;
namespace NullGarel.Sandboxnator.Settings;

[AttributeUsage(AttributeTargets.Property)]
public partial class SettingsSliderAttribute(SettingsCategory cat, string displayName, double min, double max, double step) : Attribute
{
    public SettingsCategory Category { get; } = cat;
    public string DisplayName { get; } = displayName;
    public double Min { get; } = min;
    public double Max { get; } = max;
    public double Step { get; } = step;
}