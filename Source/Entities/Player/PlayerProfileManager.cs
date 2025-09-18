using Godot;
using System.Collections.Generic;
using System;
using NullCyan.Util;
using NullCyan.Util.Log;
namespace NullCyan.Sandboxnator.Entity;

//This class will manage the player profiles, it will hold a list of player profiles and will be able to add, remove and edit them.
//This class will also be a singleton, this means that there will only be one instance of this class in the game.
//It will be a client side node, there should be a way to share this via the network somehow and make the player profiles accessible to all clients.
//Other players should be able to see the profiles of other players.

public partial class PlayerProfileManager : Singleton<PlayerProfileManager>
{
	public PlayerProfileData CurrentProfile { get; private set; }


	public override void _Ready()
	{
		if (CurrentProfile == null)
		{
			//Generate and set randomized profile
			PlayerProfileData randomizedProfile = new PlayerProfileData();
			randomizedProfile.PlayerName = FillNameField();
			randomizedProfile.PlayerColor = new Color(GD.Randf(), GD.Randf(), GD.Randf());
			CurrentProfile = randomizedProfile;
		}
	}

	private string FillNameField()
	{
		NameGenerator nameGen = NameGenerator.Create();
		GD.Randomize();
		if (GD.Randf() >= 0.5)
		{
			nameGen.UseWesternPatterns();
		}
		else
		{
			nameGen.UseSimplePatterns();
		}
		string name = nameGen.GenerateName();
		string nameCorrected = char.ToUpper(name[0]) + name.Substring(1);
		NcLogger.Log($"Randomly generated name: {nameCorrected}");
		return nameCorrected;
	}

}
