using NullCyan.Sandboxnator.Registry;

namespace NullCyan.Sandboxnator.Settings;

public interface ISettingsLoader
{
    /// <summary>
    /// Should be called on initialization as well.
    /// </summary>
    public void UpdateSettingsData();
    public void SubscribeToSettingsEventBus()
    {
        GameRegistries.Instance.OnSettingsSaved += this.UpdateSettingsData;
    }
}