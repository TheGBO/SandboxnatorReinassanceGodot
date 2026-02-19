using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Entity;
using NullCyan.Util.ComponentSystem;

public partial class PlayerSounds : AbstractComponent<Player>
{
    [Export] private AudioStreamPlayer3D streamPlayer;

    [ExportGroup("Footsteps")]
    [Export] private Array<AudioStream> genericFootsteps;

    public void PlayGenericFootstep(float footstepDelay = 0.25f)
    {
        if (streamPlayer.Playing) return;

        SceneTreeTimer footstepSoundTimer = GetTree().CreateTimer(footstepDelay);
        footstepSoundTimer.Timeout += () =>
        {
            streamPlayer.Stream = genericFootsteps.PickRandom();
            streamPlayer.Play();
        };

    }
}
