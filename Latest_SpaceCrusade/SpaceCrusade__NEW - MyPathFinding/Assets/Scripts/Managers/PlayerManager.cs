using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static PlayerManager _instance;

    ////////////////////////////////////////////////

    private static PlayerAgent _playerAgent;
    private static BasePlayerData _playerData;

    ////////////////////////////////////////////////

    private static int _playerID = 0;
    private static string _playerName = "???";
    private static int _totalPlayers = -1;
    private static int _seed = -1;

    ////////////////////////////////////////////////

    public static PlayerAgent PlayerAgent
    {
        get { return _playerAgent; }
        set { _playerAgent = value; }
    }

    public static BasePlayerData PlayerData
    {
        get { return _playerData; }
        set { _playerData = value; }
    }

    public static int PlayerID
    {
        get { return _playerID; }
        set { _playerID = value; }
    }

    public static int TotalPlayers
    {
        get { return _totalPlayers; }
        set { _totalPlayers = value; }
    }

    public static string PlayerName
    {
        get { return _playerData.name; }
    }

    public static List<int[,]> PlayerShipSmallFloorDataPART1
    {
        get { return _playerData.smallShipFloorsPART1; }
    }

    public static List<int[,]> PlayerShipSmallFloorDataPART2
    {
        get { return _playerData.smallShipFloorsPART2; }
    }

    public static List<int[,]> PlayerShipSmallVentDataPART1
    {
        get { return _playerData.smallShipVentsPART1; }
    }

    public static List<int[,]> PlayerShipSmallVentDataPART2
    {
        get { return _playerData.smallShipVentsPART2; }
    }

    public static List<UnitData> PlayerUnitData
    {
        get { return _playerData.allUnitData; }
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

    public static void SetUpPlayer()
    {
        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake

        PlayerID = _syncedVars.PlayerCount;
        LoadPlayerDataInToManager(PlayerID);
    }


    public static void LoadPlayerDataInToManager(int playerID)
    {
        BasePlayerData data = null;

        switch (playerID)
        {
            case 0:
                data = new PlayerData_00();
                break;
            case 1:
                data = new PlayerData_01();
                break;
            case 2:
                data = new PlayerData_02();
                break;
            case 3:
                data = new PlayerData_03();
                break;
            default:
                Debug.Log("SOMETHING WENT WRONG HERE: playerID: " + playerID);
                break;
        }
        PlayerData = data;
    }


    public static KeyValuePair<Vector3, Vector3> GetPlayerStartPosition(int playerID)
    {
        switch (playerID)
        {
            case 0:
                return new KeyValuePair<Vector3, Vector3>(new Vector3(-124.3f, 475, 895.9f), new Vector3(0, 90, 0));
            case 1:
                return new KeyValuePair<Vector3, Vector3>(new Vector3(11, 572, -879), new Vector3(0, 90, 0));
            case 2:
                return new KeyValuePair<Vector3, Vector3>(new Vector3(-955, 489.4f, -71), new Vector3(0, 0, 0));
            case 3:
                return new KeyValuePair<Vector3, Vector3>(new Vector3(738, 344, -210), new Vector3(0, 0, 0));
            default:
                Debug.Log("SOMETHING WENT WRONG HERE: playerID: " + playerID);
                return new KeyValuePair<Vector3, Vector3>(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        }
    }


    public static void LoadPlayersShip(Vector3 loc, Vector3 rot)
    {
        WorldBuilder._nodeBuilder.CreatePlayersShip(loc, rot, GameManager._PlayerManager.transform);
    }
}
