using Godot;
using System;
using System.Collections.Generic;

public partial class UiSoundManager : Node
{
	[Export] private AudioStreamPlayer hoverSound;
	[Export] private AudioStreamPlayer interactSound;
	public static UiSoundManager Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		TryInstallSounds();
	}

	/// <summary>
	/// Called to set up InstallSounds
	/// </summary>
	public void TryInstallSounds(Node node = null)
	{
		Node root;
		if (node == null)
		{
			root = GetTree().Root;
		}
		else
		{
			root = node;
		}
		InstallSounds(root);


	}

	/// <summary>
	/// Connect the sound to interaction buttons
	/// </summary>
	private void InstallSounds(Node node)
	{
		GD.Print("Installing sounds");
		foreach (Node child in GetAllChildrenInNode(node))
		{
			if (child is Button btn)
			{
				//workaround, button_down usually never gets connections and I use pressed instead
				bool hasConnection = btn.GetSignalConnectionList("button_down").Count > 0;
				if (hasConnection)
					return;

				btn.ButtonDown += () => { PlaySfxSound(UiSoundType.Interact); };
				btn.MouseEntered += () => { PlaySfxSound(UiSoundType.Hover); };
			}
		}
	}

	private void PlaySfxSound(UiSoundType _type)
	{
		switch (_type)
		{
			case UiSoundType.Hover:
				hoverSound.PitchScale = 1f + (float)GD.RandRange(-0.2, 0.2);
				hoverSound.Play();
				break;
			case UiSoundType.Interact:
				interactSound.PitchScale = 1f + (float)GD.RandRange(-0.2, 0.2);
				interactSound.Play();
				break;
		}
	}

	private List<Node> GetAllChildrenInNode(Node node, List<Node> nodes = null)
	{
		nodes ??= new List<Node>();
		nodes.Add(node);
		if (nodes != null)
		{
			foreach (Node kid in node.GetChildren())
			{
				nodes = GetAllChildrenInNode(kid, nodes);
			}
		}
		return nodes;
	}

	private enum UiSoundType
	{
		Hover,
		Interact
	}
}
