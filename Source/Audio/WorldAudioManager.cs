using Godot;
using Godot.Collections;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;
using System;
namespace NullGarel.Sandboxnator.Audio;

public partial class WorldAudioManager : Singleton<WorldAudioManager>
{


	/// <summary>
	/// Play 3D audio clip at a specific global position.
	/// </summary>
	/// TODO: Make this work on multiplayer and create a sfx registry
	public void PlaySoundAt(AudioStream stream, Vector3 globalPosition, float pitchScale = 1.0f)
	{
		if (stream == null) return;

		AudioStreamPlayer3D streamPlayer = new();
		SandboxnatorMain.World.AddChild(streamPlayer);
		streamPlayer.Stop();
		streamPlayer.GlobalPosition = globalPosition;
		streamPlayer.Stream = stream;
		streamPlayer.PitchScale = pitchScale;
		streamPlayer.Play();
		streamPlayer.Finished += streamPlayer.QueueFree;
	}
}
