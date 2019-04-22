using UnityEngine.Networking;

public class SyncedVars : NetworkBehaviour
{
    [SyncVar]
	public int globalSeed = -1;

    [SyncVar]
    public int playerCount = -1;

	public int GlobalSeed
	{
		get { return globalSeed; }
		set { globalSeed = value; }
	}

	public int PlayerCount
	{
		get { return playerCount; }
		set { playerCount = playerCount + value; }
	}
}
