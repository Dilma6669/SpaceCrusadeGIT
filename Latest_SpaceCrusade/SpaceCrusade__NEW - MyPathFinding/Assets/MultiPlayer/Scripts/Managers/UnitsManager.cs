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

    public static Dictionary<int, Dictionary<int, UnitScript>> _unitObjects;

    private static UnitScript _activeUnit = null;

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
        _unitBuilder = transform.Find("UnitBuilder").GetComponent<UnitBuilder>();

        _unitObjects = new Dictionary<int, Dictionary<int, UnitScript>> ();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void LoadPlayersUnits(Vector3 worldStartLoc)
    {
        List<UnitData> units = PlayerManager.PlayerUnitData;

        int unitCount = 0;
        foreach (UnitData unit in units)
        {
            unitCount += 1;

            Vector3 localStart = unit.UnitStartingLocalLoc;
            Vector3 worldStart = new Vector3(localStart.x + worldStartLoc.x, localStart.y + worldStartLoc.y, localStart.z + worldStartLoc.z);

            bool lastUnit = (unitCount == units.Count) ? true : false;
            CreateUnitOnNetwork(unit, worldStart, worldStartLoc, lastUnit);
        }
    }

    ////////////////////////////////////////////////

    public static void AllPlayerUnitsHaveBeenLoaded()
    {
        AssignCameraToActiveUnit();
    }

    ////////////////////////////////////////////////

    public static void AddUnitToGame(int playerContID, int unitID, UnitScript unit)
    {
        if (_unitObjects.ContainsKey(playerContID))
        {
            Dictionary<int, UnitScript> unitList = _unitObjects[playerContID];

            if (!unitList.ContainsKey(unitID))
            {
                unitList.Add(unitID, unit);
            }
            else
            {
                Debug.LogError("Trying to assign a unit twice");
            }
        }
        else
        {
            Dictionary<int, UnitScript> unitList = new Dictionary<int, UnitScript>();
            unitList.Add(unitID, unit);
            _unitObjects.Add(playerContID, unitList);
        }
    }

    ////////////////////////////////////////////////

    public static void SetUnitActive(bool onOff, int playerContID, int unitID)
    {
        UnitScript unit = _unitObjects[playerContID][unitID];

        if (unit == null)
        {
            Debug.LogError("SetUnitActive ERROR unit == null");
            return;
        }

        if (_activeUnit == null)
        {
            _activeUnit = unit;
        }

        if (onOff)
        {
            if (_activeUnit.NetID.Value != unit.NetID.Value)
            {
                _activeUnit.DeActivateUnit();
            }

            print("SetUnitActive <<<<<<<<<<<<<<<<<<<< unitId: " + unit.NetID.Value);

            _activeUnit = unit;
            _activeUnit.ActivateUnit();

            // _locationManager.DebugTestPathFindingNodes(_activeUnit);
        }
        else
        {
            if (_activeUnit.NetID.Value != unit.NetID.Value)
            {
                Debug.LogError("should not be here Unit is active and another unit is tying to get turned off");
            }
            else
            {
                _activeUnit.DeActivateUnit();
                _activeUnit = null;
            }
        }
    }

    ////////////////////////////////////////////////

    public static void AssignCameraToActiveUnit()
    {
        CameraManager.SetCamToOrbitUnit(_activeUnit);
        LayerManager.ChangeCameraLayer(_activeUnit.CubeUnitIsOn);
    }

    ////////////////////////////////////////////////

    public static void MakeActiveUnitMove_CLIENT(Vector3 posToMoveTo)
    {
        if (_activeUnit)
        {
            List<Vector3> movePath = MovementManager.SetUnitsPath(_activeUnit, _activeUnit.CubeUnitIsOn.CubeStaticLocVector, posToMoveTo);

            int[] pathInts = DataManipulation.ConvertVectorsIntoIntArray(movePath);

            int unitID = (int)_activeUnit.netId.Value;
            MovementManager.CreatePathFindingNodes_CLIENT(_activeUnit, unitID, movePath);

            NetWorkManager.NetworkAgent.CmdTellServerToMoveUnit(PlayerManager.PlayerAgent.NetID, _activeUnit.NetID, pathInts);

        }
    }

    ////////////////////////////////////////////////

    private static void CreateUnitOnNetwork(UnitData unitData, Vector3 worldStart, Vector3 nodeID, bool lastUnit)
    {
        int playerID = PlayerManager.PlayerID;
        NetWorkManager.NetworkAgent.CmdTellServerToSpawnPlayerUnit(PlayerManager.PlayerAgent.NetID, unitData, playerID, worldStart, nodeID, lastUnit);
    }

}
