using Godot;
using NullGarel.Sandboxnator.Network;
using System;
namespace NullGarel.Sandboxnator.UI;

public partial class MultiplayerMenu : Control, IUiSignalLoader
{
	[ExportCategory("Network inputs")]
	[Export] private SpinBox _portInput;
	[Export] private LineEdit _ipAddressInput;
	[Export] private Button _hostBtn;
	[Export] private Button _joinBtn;
	[Export] private Button _mainMenuBtn;
	[ExportCategory("Screens")]
	[Export] private Control _connectionMenu;
	[Export] private Control _loadingScreen;
	[Export] private ProgressBar _timeoutProgress;
	[Export] private Button _cancelConnectionBtn;

	public override void _Ready()
	{
		ConnectUISignals();
		_timeoutProgress.MaxValue = NetworkManager.ConnectionTimeoutLimit;
		NetworkManager.Instance.ConnectionFailed += OnConnectionFailed;
		NetworkManager.Instance.ServerDisconnected += OnConnectionFailed;
	}

	private void OnConnectionFailed()
	{
		SandboxnatorMain.Instance.ActivateWorldMenu();
	}

	public override void _Process(double delta)
	{
		if (!Visible)
			return;

		NetworkManager network = NetworkManager.Instance;
		if (network == null)
			return;

		_timeoutProgress.Value = NetworkManager.ConnectionTimeoutLimit - NetworkManager.Instance.ElapsedConnectionTime;
		_loadingScreen.Visible = NetworkManager.Instance.IsConnecting;
		bool showMenu = true;

		if (NetworkManager.Instance.peer != null)
		{
			MultiplayerPeer.ConnectionStatus status = NetworkManager.Instance.peer.GetConnectionStatus();
			showMenu = status != MultiplayerPeer.ConnectionStatus.Connected &&
					status != MultiplayerPeer.ConnectionStatus.Connecting;
		}

		_connectionMenu.Visible = showMenu;
	}

	public void ConnectUISignals()
	{
		_cancelConnectionBtn.Pressed += () =>
		{
			NetworkManager.Instance.QuitConnection();
		};

		_hostBtn.Pressed += () =>
		{
			if (NetworkManager.Instance == null)
			{
				GD.PushError("MultiplayerMenu: NetworkManager.Instance is null when attempting to host.");
				return;
			}

			NetworkManager.Instance.HostGame(
				(int)_portInput.Value,
				false);
		};

		_joinBtn.Pressed += () => NetworkManager.Instance.JoinGame((int)_portInput.Value, _ipAddressInput.Text);
		_mainMenuBtn.Pressed += () => SandboxnatorMain.Instance.ActivateMainMenu();
	}

    public void DisconnectUISignals()
    {
        throw new NotImplementedException();
    }
}
