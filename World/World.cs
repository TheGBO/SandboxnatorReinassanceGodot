using Godot;
using Godot.Collections;
using NullCyan.Sandboxnator.Building;
using NullCyan.Util;
using System;
using System.Collections.Generic;
using NullCyan.Sandboxnator.Entity;
namespace NullCyan.Sandboxnator.WorldAndScenes;

/// <summary>
/// Class that holds the world scene data
/// </summary>
public partial class World : Singleton<World>
{
	public Action<long> OnPlayerJoin;
	public List<Snapper> snappers = new List<Snapper>();

	[Export] public Node3D networkedEntities;


	public Vector3 GetNearestSnapper(Vector3 referential, float maxRange)
	{
		foreach (Snapper snapper in snappers)
		{
			if ((referential.DistanceTo(snapper.GlobalPosition) <= maxRange) && !snapper.InsideBody)
			{
				return snapper.GlobalPosition;
			}
		}
		return referential;
	}


	/// <summary>
	/// Expected and designed to run on the server for now
	/// </summary>
	/// <returns>An array of the current players.</returns>
	public Array<Player> GetPlayers()
	{
		Array<Player> players = new Array<Player>();
		foreach (Node e in networkedEntities.GetChildren())
		{
			if (e is Player)
			{
				players.Add((Player)e);
			}
			GD.Print();
		}
		return players;
	}

	public PlayerProfileData GetPlayerProfileDataByID(long id)
	{
		foreach (Player player in GetPlayers())
		{
			GD.Print("data: " + player.profileData);
			if (player.componentHolder.entityId == id)
			{
				return player.profileData;
			}
		}
		return null;
	}
}
