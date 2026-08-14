using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.IO;
using NathanHoad;
using NullGarel.Util.Log;
using NullGarel.Sandboxnator.UI;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Sandboxnator;
namespace NullGarel.UI;

public partial class SettingsMenu : Control, IUiSignalLoader
{
	private GameSettingsData _currentGameSettings = new();
	[ExportCategory("Main buttons")]
	[Export] private Button _acceptBtn;
	[Export] private Button _resetToDefaultsBtn;

	[ExportCategory("Controls settings")]
	[Export] private Slider _fovSlider;
	[Export] private Slider _lookSensitivitySlider;
	[Export] private GameSettingsData _defaultSettings;

	public override void _EnterTree()
	{
		UIFromSettings();
		//InputActionsDebug();
		ConnectUISignals();
	}

	public void ConnectUISignals()
	{
		var greg = GameRegistries.Instance;

		greg.OnSettingsChanged += UIFromSettings;
		VisibilityChanged += UIFromSettings;
		_acceptBtn.Pressed += () =>
		{
			SettingsFromUI();
			SandboxnatorMain.Instance.ActivateMainMenu();
		};
		_resetToDefaultsBtn.Pressed += () =>
		{
			greg.SettingsData = _defaultSettings;
		};
	}

	/// <summary>
	/// Writes the user selected settings into the filesystem.
	/// </summary>
	public void SettingsFromUI()
	{
		var greg = GameRegistries.Instance;

		_currentGameSettings.FieldOfView = _fovSlider.Value;
		_currentGameSettings.LookSensitivity = _lookSensitivitySlider.Value;

		greg.SettingsData = _currentGameSettings;
	}

	/// <summary>
	/// Reads the registry and updates the UI to display settings info.
	/// </summary>
	public void UIFromSettings()
	{
		var greg = GameRegistries.Instance;
		_fovSlider.Value = greg.SettingsData.FieldOfView;
		_lookSensitivitySlider.Value = greg.SettingsData.LookSensitivity;

	}

	//TODO: Keybind remapping system.
	private void InputActionsDebug()
	{
		Array<StringName> actions = InputMap.GetActions();
		foreach (StringName action in actions)
		{
			string actionName = action.ToString();
			if (actionName.StartsWith("ui_"))
			{
				continue;
			}
			Array<InputEvent> eventsForAction = InputHelper.GetKeyboardInputsForAction(action);
			foreach (var e in eventsForAction)
			{
				GD.PrintRich($"{action} :: {e.AsText()}");
			}
		}
	}

}
