using Godot;
using NullGarel.Util.Log;
using System;
namespace NullGarel.Sandboxnator;

/// <summary>
/// An autoload script to manage os events like closing the app.
/// </summary>
public partial class ApplicationManager : Node
{
	public override void _Ready()
	{
		GetTree().Root.CloseRequested += OnCloseRequested;
	}

	private void OnCloseRequested()
	{
		NcLogger.Log("ApplicationManager :: close requested.");
		GetTree().Quit();
	}

	public override void _ExitTree()
	{
		GetTree().Root.CloseRequested -= OnCloseRequested;
	}
}
