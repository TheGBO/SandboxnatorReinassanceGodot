using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.IO;
using NathanHoad;
using NullGarel.Util.Log;
using NullGarel.Sandboxnator.UI;
namespace NullGarel.UI;

public partial class SettingsMenu : Control
{
    [Export] private Slider FovSlider;
    [Export] private Slider LookSensitivitySlider;

    public override void _EnterTree()
    {
        UiSoundManager.Instance.TryInstallSounds();
        UIFromSettings();
        InputActionsDebug();
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
                string eventName = e.AsText();

                GD.PrintRich($"{action} :: {e.AsText()}");

            }
        }
    }

    public void SettingsFromUI()
    {
        var greg = GameRegistries.Instance;
        //read from ui and put into the registry
        greg.SettingsData.FieldOfView = FovSlider.Value;
        greg.SettingsData.LookSensitivity = LookSensitivitySlider.Value;

        SaveLoader.Instance.WriteResource(SaveFolder.Config, greg.UserSettingsName, greg.SettingsData);
        greg.OnSettingsSaved?.Invoke();
    }

    //TODO: also call this from "visibility changed"
    public void UIFromSettings()
    {
        var greg = GameRegistries.Instance;
        //load from the registry and put into the ui
        FovSlider.Value = greg.SettingsData.FieldOfView;
        LookSensitivitySlider.Value = greg.SettingsData.LookSensitivity;

    }

    public void ResetDefaults()
    {

    }

    public void _on_accept_btn_pressed()
    {
        SettingsFromUI();
    }

}
