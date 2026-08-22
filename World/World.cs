using Godot;
using Godot.Collections;
using NullGarel.Sandboxnator.Building;
using System;
using System.Collections.Generic;
using NullGarel.Sandboxnator.Entity;
using NullGarel.Sandboxnator.Registry;
using NullGarel.Util.Log;
using NullGarel.Sandboxnator.Item;
using NullGarel.Util.GodotHelpers;
namespace NullGarel.Sandboxnator.WorldAndScenes;

/// <summary>
/// Class that holds the world scene data
/// </summary>
public partial class World : Node3D
{
	public Action<long> OnPlayerJoin;
	public List<Snapper> snappers = [];

	[Export] private Node3D _networkedEntities;
	public Node3D NetworkedEntities => _networkedEntities;

	[Export] public MultiplayerSpawner BuildingSpawner { get; private set; }
	[Export] public MultiplayerSpawner PlayerSpawner { get; private set; }

	private readonly HashSet<string> addedBuildingScenes = [];

	public override void _EnterTree()
	{
		AddBuildingScenesToSpawnList();
		BuildingSpawner.SpawnFunction = new Callable(this, nameof(SpawnBuilding));
	}

	private void AddBuildingScenesToSpawnList()
	{
		//commit building items to the auto spawn list
		foreach (PackedScene buildingScene in GameRegistries.Instance.BuildingRegistry.GetAllValues())
		{
			if (buildingScene == null)
			{
				NcLogger.Log("Found null buildingScene!");
				continue;
			}
			string resPath = buildingScene.ResourcePath;
			if (addedBuildingScenes.Add(resPath))
			{
				BuildingSpawner.AddSpawnableScene(resPath);
			}
		}
	}

	private Node SpawnBuilding(Variant data)
	{
		var spawnData = DictPack.Deserialize<BuildingSpawnData>((Dictionary)data);

		PackedScene scene = GameRegistries.Instance.BuildingRegistry.Get(spawnData.ItemId);
		Placeable building = (Placeable)scene.Instantiate();

		building.ItemData = (PlaceableItemData)GameRegistries.Instance.ItemRegistry.Get(spawnData.ItemId);
		building.Position = spawnData.Position;
		building.Rotation = spawnData.Rotation;
		building.Name = Guid.NewGuid().GetHashCode().ToString();

		return building;
	}


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
		Array<Player> players = [];
		foreach (Node e in NetworkedEntities.GetChildren())
		{
			if (e is Player player)
			{
				players.Add(player);
			}
		}
		return players;
	}

	public Player GetPlayerById(long id)
	{
		foreach (Player player in GetPlayers())
		{
			if (player.componentHolder.entityId == id)
			{
				return player;
			}
		}
		return null;
	}

	public PlayerProfileData GetPlayerProfileDataByID(long id)
	{
		return GetPlayerById(id)?.ProfileData;
	}
}
