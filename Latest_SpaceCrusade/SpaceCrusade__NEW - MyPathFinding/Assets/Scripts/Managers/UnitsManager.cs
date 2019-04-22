using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static UnitsManager _instance;

    private static UnitsAgent _unitsAgent;

    ////////////////////////////////////////////////

    public static UnitBuilder _unitBuilder;

    ////////////////////////////////////////////////

    public static UnitsAgent UnitsAgent
    {
        get { return _unitsAgent; }
        set { _unitsAgent = value; }
    }

    ////////////////////////////////////////////////

    private static UnitScript _activeUnit = null;

    public static List<GameObject> unitObjects = new List<GameObject>();
    public static List<UnitScript> unitScripts = new List<UnitScript>();

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

    void Start()
    {
        _unitBuilder = GameObject.Find("UnitBuilder").GetComponent<UnitBuilder>();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void LoadPlayersUnits(Vector3 worldStartLoc)
    {
        List<UnitData> units = PlayerManager.PlayerUnitData;

        foreach (UnitData unit in units)
        {
            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldStartLoc.x, localStart.y + worldStartLoc.y, localStart.z + worldStartLoc.z);

            CreateUnitOnNetwork(unit, worldStart, worldStartLoc);
        }
    }


    private static void CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart, Vector3 nodeID)
    {
        int playerID = PlayerManager.PlayerID;
        NetWorkManager.NetworkAgent.CmdTellServerToSpawnPlayerUnit(PlayerManager.PlayerAgent.NetID, unitData, playerID, worldStart, nodeID);
    }


    public static void SetUnitActive(bool onOff, UnitScript unit = null)
    {
        if (onOff)
        {
            if (_activeUnit)
            {
                _activeUnit.ActivateUnit(false);
            }
            // ._cubeManager.SetCubeActive (false);
            CameraManager.SetCamToOrbitUnit(unit.transform);
            LayerManager.ChangeCameraLayer(unit.CubeUnitIsOn);
            _activeUnit = unit;
            // ._locationManager.DebugTestPathFindingNodes(_activeUnit);
        }
        else
        {
            _activeUnit = null;
        }
    }

    public static void MakeActiveUnitMove(KeyValuePair<Vector3, Vector3> posRot)
    {
        if (_activeUnit)
        {
            NetWorkManager.NetworkAgent.CmdTellServerToMoveUnit(PlayerManager.PlayerAgent.NetID, _activeUnit.NetID, posRot.Key, posRot.Value);
        }
    }

    public static void MakeUnitRecalculateMove(UnitScript unit, KeyValuePair<Vector3, Vector3> posRot)
    {
        Debug.Log("recalulating from unitsAgent");
        NetWorkManager.NetworkAgent.CmdTellServerToMoveUnit(PlayerManager.PlayerAgent.NetID, unit.NetID, posRot.Key, posRot.Value);
    }

    /*
    public static void SetUpUnitForPlayer(GameObject unit)
    {
        Debug.Log("fucken unit 3: " + unit);
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.CubeUnitIsOn =  ._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
        unitScript.PlayerControllerID = _playerManager.PlayerID;
        Debug.Log("fucken unitScript.PlayerControllerID 1: " + unitScript.PlayerControllerID);
    }
    */


    /*
    public static void AssignUniqueLayerToUnits()
    {
        string layerStr = "Player0" +  ._playerManager._playerAgent.PlayerUniqueID.ToString() + "Units";
        gameObject.layer = LayerMask.NameToLayer(layerStr);

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerStr);
        }
    }
    */


}
