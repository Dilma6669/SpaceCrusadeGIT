using UnityEngine;
using UnityEngine.Networking;


public class PlayerAgent : NetworkBehaviour
{
    ////////////////////////////////////////////////

    NetworkInstanceId _netID;
    int _totalPlayers = -1;

    ////////////////////////////////////////////////

    public NetworkInstanceId NetID
    {
        get { return _netID; }
        set { _netID = value; }
    }

    public int TotalPlayers
    {
        get { return _totalPlayers; }
        set { _totalPlayers = value; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // Use this for initialization
    void Awake()
    {
    }

    // Need this Start()
    void Start()
    {
        if (!isLocalPlayer) return;
        PlayerManager.PlayerAgent = this;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public override void OnStartLocalPlayer()
    {
        Start();
        if (!isLocalPlayer) return;
        Debug.Log("A network Player object has been created");

        transform.SetParent(GameManager._PlayerManager.transform);
        PlayerManager.SetUpPlayer();

        NetID = GetComponent<NetworkIdentity>().netId;
        GetComponent<NetworkAgent>().CmdAddPlayerToSession(NetID);

        UIManager.SetUpPlayersGUI(PlayerManager.PlayerID);
        CameraManager.SetUpCameraAndLayers(PlayerManager.PlayerID, GetComponent<CameraAgent>());

        PlayerManager.LoadPlayersShip(transform.position, transform.localEulerAngles);

        WorldManager.BuildWorldForClient();
    }


    public void UpdatePlayerCount(int count)
    {
        TotalPlayers = count;
        UIManager.UpdateTotalPlayersGUI(count);
    }

    public void SetUpPlayerStartPosition(Vector3 camPos, Quaternion camRot)
    {
        transform.position = camPos;
        transform.rotation = camRot;
    }


}
