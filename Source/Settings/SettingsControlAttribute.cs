
using System;
namespace NullGarel.Sandboxnator.Settings;



[AttributeUsage(AttributeTargets.Property)]
public abstract partial class SettingsControlAttribute
(
    SettingsCategory category,
    string displayName
) : Attribute
{
    public SettingsCategory Category { get; } = category;
    public string DisplayName { get; } = displayName;
}