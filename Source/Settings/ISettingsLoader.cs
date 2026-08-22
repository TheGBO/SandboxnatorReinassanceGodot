namespace NullGarel.Sandboxnator.Settings;

public interface ISettingsLoader
{
    /// <summary>
    /// Should be called on initialization as well.
    /// </summary>
    public void UpdateSettingsData();

}