using Godot;
using System;
using System.Collections.Generic;
using NullGarel.Util;
using NullGarel.Util.GodotHelpers;
namespace NullGarel.Sandboxnator.UI;

public partial class UiSoundManager : Singleton<UiSoundManager>
{
	[Export] private AudioStreamPlayer _hoverSound;
	[Export] private AudioStreamPlayer _interactSound;
	[Export] private AudioStreamPlayer _popUpSound;

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
		foreach (Node child in NodeUtils.GetAllChildrenInNode(node))
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

	public void PlaySfxSound(UiSoundType soundType)
	{
		switch (soundType)
		{
			case UiSoundType.Hover:
				_hoverSound.PitchScale = 1f + (float)GD.RandRange(-0.25, 0.25);
				_hoverSound.Play();
				break;
			case UiSoundType.Interact:
				_interactSound.Play();
				break;
			case UiSoundType.PopUp:
				_popUpSound.Play();
				break;

		}
	}
}
