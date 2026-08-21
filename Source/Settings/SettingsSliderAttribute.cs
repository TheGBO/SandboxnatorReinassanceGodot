using System;

namespace NullGarel.Sandboxnator.Settings;

/// <summary>
/// A decimal number constrained by a range and incremented by a step.
/// Represents a visual slider in the UI.
/// </summary>
/// <param name="category">The tab where it will be rendered</param>
/// <param name="displayName">the display name</param>
/// <param name="min">minimum value</param>
/// <param name="max">maximum value</param>
/// <param name="step">incremental step</param>
[AttributeUsage(AttributeTargets.Property)]
public partial class SettingsSliderAttribute
(
    SettingsCategory category,
    string displayName,
    double min,
    double max,
    double step,
    string formatting = "F0"
) : SettingsControlAttribute(category, displayName)
{
    public double Min { get; } = min;
    public double Max { get; } = max;
    public double Step { get; } = step;
    public string Formatting { get; } = formatting;
}