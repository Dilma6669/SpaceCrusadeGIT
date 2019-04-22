using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : NetworkManager
{
    ////////////////////////////////////////////////

    private static NetWorkManager _instance;

    ////////////////////////////////////////////////

    private static NetworkAgent _networkAgent;

    ////////////////////////////////////////////////

    public static NetworkAgent NetworkAgent
    {
        get { return _networkAgent; }
        set { _networkAgent = value; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // called on the SERVER when a client connects
    public override void OnServerConnect(NetworkConnection Conn)
	{
        Debug.Log("NETWORKMANAGER: Client Connect!! Con: " + Conn.hostId);

        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake

        if (Conn.hostId == -1)
        {
            int globalSeed = Random.Range(0, 100);
            Random.InitState(globalSeed);
            _syncedVars.GlobalSeed = globalSeed;
        }

        _syncedVars.PlayerCount = 1;
	}

}
