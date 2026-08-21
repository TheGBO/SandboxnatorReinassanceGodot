using Godot;
using System;
using NullGarel.Util.ComponentSystem;
namespace NullGarel.Sandboxnator.Entity;

public partial class HandSway : AbstractComponent<Player>
{
    [Export] private float _swaySpeed = 3.0f;
    [Export] private Node3D _hand;
    [Export] private Vector3 _rightSway = new(0, 0.56f, 0);
    [Export] private Vector3 _leftSway = new(0, -0.56f, 0);
    [Export] private float _swayThreshold = 5.0f;

    private PlayerInput _playerInput;

    public override void _Ready()
    {
        _playerInput = GetComponent<PlayerInput>();
    }



    public override void _Process(double delta)
    {
        if (!IsMultiplayerAuthority()) return;

        if (_playerInput.LookVector.X > _swayThreshold)
        {
            _hand.Rotation = _hand.Rotation.Lerp(_rightSway, _swaySpeed * (float)delta);
        }
        else if (_playerInput.LookVector.X < -_swayThreshold)
        {
            _hand.Rotation = _hand.Rotation.Lerp(_leftSway, _swaySpeed * (float)delta);
        }
        else
        {
            _hand.Rotation = _hand.Rotation.Lerp(Vector3.Zero, _swaySpeed * (float)delta);
        }
    }
}
