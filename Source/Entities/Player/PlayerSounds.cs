using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Entity;
using NullGarel.Util.ComponentSystem;

public partial class PlayerSounds : AbstractComponent<Player>
{
    [Export] private AudioStreamPlayer3D _streamPlayer;

    [ExportGroup("Footsteps")]
    [Export] private Array<AudioStream> _genericFootsteps;

    public void PlayGenericFootstep(float footstepDelay = 0.25f)
    {
        if (_streamPlayer.Playing) return;

        SceneTreeTimer footstepSoundTimer = GetTree().CreateTimer(footstepDelay);
        footstepSoundTimer.Timeout += () =>
        {
            _streamPlayer.Stream = _genericFootsteps.PickRandom();
            _streamPlayer.Play();
        };

    }
}
