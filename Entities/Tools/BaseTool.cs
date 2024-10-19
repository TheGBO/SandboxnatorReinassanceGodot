using Godot;
using System;

[Tool]
public partial class BaseTool : Node3D
{
	/// <summary>
	/// abbreviation for Player Tool Use
	/// </summary>
	public PlayerToolUse Ptu { get; set; }
	[Export] public bool animateHand;

	public virtual void UseTool(ToolUsageArgs args)
	{

	}
}
