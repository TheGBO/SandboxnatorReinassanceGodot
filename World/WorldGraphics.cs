using Godot;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Sandboxnator.Settings;

namespace NullGarel.Sandboxnator.WorldAndScenes;
//class responsiple for getting the graphics settings and applying it to world envinrornementnent
//RUNS ON CLIeNT
[GlobalClass]
public partial class WorldGraphics : Node, ISettingsLoader
{
	[Export] private WorldEnvironment _worldEnvironment;

	public override void _Ready()
	{
		UpdateSettingsData();
		GameRegistries.Instance.OnSettingsChanged += UpdateSettingsData;
	}

	public void UpdateSettingsData()
	{
		GameSettingsData d = GameRegistries.Instance.SettingsData;
		var env = _worldEnvironment.Environment;
		env.SsaoEnabled = d.AmbientOcclusion;
		env.SsrEnabled = d.SSR;
		env.SsilEnabled = d.SSIL;
		env.FogEnabled = d.Fog;
		env.GlowBloom = d.Bloom;

	}
}
