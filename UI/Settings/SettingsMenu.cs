using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using NullGarel.Sandboxnator;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;
using NullGarel.Sandboxnator.UI;

namespace NullGarel.UI;

public partial class SettingsMenu : Control, IUiSignalLoader
{
	[ExportCategory("Tabs")]
	[Export] private VBoxContainer _controlsSettingsContainer;
	[Export] private VBoxContainer _graphicsSettingsContainer;
	[Export] private VBoxContainer _audioSettingsContainer;

	[ExportCategory("Buttons")]
	[Export] private Button _acceptBtn;
	[Export] private Button _resetToDefaultsBtn;
	[Export] private GameSettingsData _defaultSettings;

	private readonly Dictionary<PropertyInfo, Slider> _boundSliders = [];
	private GameSettingsData _currentGameSettings = new();

	public override void _Ready()
	{
		GenerateDynamicSettings();
		ConnectUISignals();
		UIFromSettings();
	}

	public override void _ExitTree()
	{
		DisconnectUISignals();
	}

	private void GenerateDynamicSettings()
	{
		PropertyInfo[] properties = typeof(GameSettingsData).GetProperties(
			BindingFlags.Public | BindingFlags.Instance
		);

		GenerateSliders(properties);
	}

	private void GenerateSliders(PropertyInfo[] properties)
	{
		foreach (PropertyInfo prop in properties)
		{
			SettingsSliderAttribute attr = prop.GetCustomAttribute<SettingsSliderAttribute>();
			if (attr == null) continue;

			HBoxContainer row = new();

			Label titleLabel = new()
			{
				Text = attr.DisplayName,
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				HorizontalAlignment = HorizontalAlignment.Center,
				SizeFlagsStretchRatio = 0.5f
			};

			HSlider slider = new()
			{
				MinValue = attr.Min,
				MaxValue = attr.Max,
				Step = attr.Step,
				CustomMinimumSize = new Vector2(200, 0),
				SizeFlagsHorizontal = SizeFlags.ExpandFill
			};

			Label valueLabel = new()
			{
				Text = slider.Value.ToString("F0"),
				CustomMinimumSize = new Vector2(50, 0),
				SizeFlagsHorizontal = SizeFlags.ExpandFill,
				HorizontalAlignment = HorizontalAlignment.Center,
				SizeFlagsStretchRatio = 0.25f

			};

			slider.ValueChanged += value =>
			{
				valueLabel.Text = value.ToString("F0");
			};

			row.AddChild(titleLabel);
			row.AddChild(slider);
			row.AddChild(valueLabel);

			VBoxContainer target = attr.Category switch
			{
				SettingsCategory.Audio => _audioSettingsContainer,
				SettingsCategory.Controls => _controlsSettingsContainer,
				SettingsCategory.Graphics => _graphicsSettingsContainer,
				_ => throw new ArgumentOutOfRangeException(
					nameof(attr.Category),
					attr.Category,
					$"Unhandled category target: {attr.Category}"
				)
			};

			target.AddChild(row);
			_boundSliders[prop] = slider;
		}
	}

	public void ConnectUISignals()
	{
		var greg = GameRegistries.Instance;
		greg.OnSettingsChanged += UIFromSettings;

		_acceptBtn.Pressed += OnAcceptPressed;
		_resetToDefaultsBtn.Pressed += OnResetPressed;
	}

	public void DisconnectUISignals()
	{
		var greg = GameRegistries.Instance;
		if (greg != null)
		{
			greg.OnSettingsChanged -= UIFromSettings;
		}

		_acceptBtn.Pressed -= OnAcceptPressed;
		_resetToDefaultsBtn.Pressed -= OnResetPressed;
	}

	private void OnAcceptPressed()
	{
		SettingsFromUI();
		SandboxnatorMain.Instance.ActivateMainMenu();
	}

	private void OnResetPressed()
	{
		GD.Print(_defaultSettings);
		GameRegistries.Instance.SettingsData = (GameSettingsData)_defaultSettings.Duplicate();
	}

	#region Settings I/O
	public void SettingsFromUI()
	{
		var greg = GameRegistries.Instance;

		foreach (var (prop, slider) in _boundSliders)
		{
			object convertedValue = Convert.ChangeType(slider.Value, prop.PropertyType);
			prop.SetValue(_currentGameSettings, convertedValue);
		}

		greg.SettingsData = _currentGameSettings;
	}

	public void UIFromSettings()
	{
		var greg = GameRegistries.Instance;
		if (greg.SettingsData == null) return;

		_currentGameSettings = greg.SettingsData;

		foreach (var (prop, slider) in _boundSliders)
		{
			object value = prop.GetValue(_currentGameSettings);
			if (value != null)
			{
				slider.Value = Convert.ToDouble(value);
			}
		}
	}

	#endregion
}