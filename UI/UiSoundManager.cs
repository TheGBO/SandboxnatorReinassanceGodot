using Godot;
using System;
using System.Collections.Generic;
using NullCyan.Util;
namespace NullCyan.Sandboxnator.UI;

public partial class UiSoundManager : Singleton<UiSoundManager>
{
	[Export] private AudioStreamPlayer hoverSound;
	[Export] private AudioStreamPlayer interactSound;
	[Export] private AudioStreamPlayer popUpSound;

	public override void _Ready()
	{
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
		//GD.Print("Installing sounds");
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

	public void PlaySfxSound(UiSoundType _type)
	{
		switch (_type)
		{
			case UiSoundType.Hover:
				hoverSound.PitchScale = 1f + (float)GD.RandRange(-0.2, 0.2);
				hoverSound.Play();
				break;
			case UiSoundType.Interact:
				interactSound.Play();
				break;
			case UiSoundType.PopUp:
				popUpSound.Play();
				break;

		}
	}

	//TODO: This function is so useful that it should be added to utils later in order to avoid code duplications.
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
}
