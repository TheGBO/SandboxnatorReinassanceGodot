using Godot;

public partial class PlayerManager : NetworkedEntityManager<Player>
{
	public int LocalPlayerId { get; private set; } = -1;
	[Export] public PackedScene playerScene;

	public void SetLocalPlayerId(int id) => LocalPlayerId = id;
	public bool IsLocalPlayer(int id) => id == LocalPlayerId;

	public static new PlayerManager Instance => (PlayerManager)Singleton<NetworkedEntityManager<Player>>.Instance;
	
	public Player SpawnPlayer(int id, Vector3 position, Quaternion rotation)
    {
        if (HasEntity(id))
            return GetEntity(id);

        Player playerInstance = playerScene.Instantiate<Player>();
        playerInstance.Name = $"Player_{id}";
        GD.Print(playerInstance.Position);
        playerInstance.Position = position;
        playerInstance.Quaternion = rotation;

        AddEntity(id, playerInstance);
        return playerInstance;
    }

}
