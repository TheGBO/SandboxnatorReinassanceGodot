
using System;
namespace NullGarel.Sandboxnator.Settings;

/// <summary>
/// A boolean.xd. too lazy to type the rest.
/// </summary>
/// <param name="category">The tab where it will be rendered</param>
/// <param name="displayName">the display name</param>

[AttributeUsage(AttributeTargets.Property)]
public partial class SettingsToggleAttribute
(
    SettingsCategory category,
    string displayName
) : SettingsControlAttribute(category, displayName)
{

}