using Godot;
using Godot.Collections;
using NullGarel.Util.ComponentSystem;
using NullGarel.Util.GodotHelpers;
using System;
namespace NullGarel.Sandboxnator.Audio;

public partial class WorldAudioManager : Singleton<WorldAudioManager>
{
	[Export] private int _poolSize = 32;
	private Array<AudioStreamPlayer3D> _streamPlayerPool = new();
	private int _nextPoolIndex = 0;

	public override void _Ready()
	{
		Instance = this;
		for (int i = 0; i < _poolSize; i++)
		{
			var streamPlayer = new AudioStreamPlayer3D
			{
				Name = $"SpatialAudioPlayer_{i}",
				MaxDistance = 16f,
				UnitSize = 8f
			};
			AddChild(streamPlayer);
			_streamPlayerPool.Add(streamPlayer);
		}
	}

	/// <summary>
	/// Play 3D audio clip at a specific global position.
	/// </summary>
	public void PlaySoundAt(AudioStream stream, Vector3 globalPosition, float pitchScale = 1.0f)
	{
		if (stream == null) return;

		var streamPlayer = _streamPlayerPool[_nextPoolIndex];
		_nextPoolIndex = (_nextPoolIndex + 1) % _poolSize;

		streamPlayer.Stop();
		streamPlayer.GlobalPosition = globalPosition;
		streamPlayer.Stream = stream;
		streamPlayer.PitchScale = pitchScale;
		streamPlayer.Play();
	}
}
