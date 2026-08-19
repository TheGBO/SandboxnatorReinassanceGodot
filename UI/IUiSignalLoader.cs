namespace NullGarel.Sandboxnator.UI;

/// <summary>
/// This interface is sort of useless, but semantically it serves the purpose of
/// being a contract. UI controls tha hold buttons and other event-based controls
/// are bound to implement this as a contract to connect and dispose such events.
/// </summary>
public interface IUiSignalLoader
{
    public void ConnectUISignals();
    public void DisconnectUISignals();
}