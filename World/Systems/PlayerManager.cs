public partial class PlayerManager : NetworkedEntityManager<Player>
{
	public int LocalPlayerId { get; private set; } = -1;

	public void SetLocalPlayerId(int id) => LocalPlayerId = id;
	public bool IsLocalPlayer(int id) => id == LocalPlayerId;
}
