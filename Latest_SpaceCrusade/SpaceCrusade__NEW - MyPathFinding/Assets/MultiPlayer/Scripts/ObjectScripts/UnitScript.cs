using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UnitScript : NetworkBehaviour
{
    ////////////////////////////////////////////////

    // GamePlay data
    private List<CubeLocationScript> _pathFindingNodes;
    private bool _unitActive;
    // Visual
    private Renderer[] _rends;
    // for the player+camera to pivot around
    GameObject _playerPivot;

    // Unit stats
    private int _unitModel;
    private bool _unitCanClimbWalls;
    private int[] _unitCombatStats;
    private Vector3 _startingWorldLoc;
    private int _playerControllerId;
    private NetworkInstanceId _netID;
    private Vector3 _nodeIDUnitIsOn;
    public CubeLocationScript _cubeUnitIsOn;


    private int _pathNodeCount;


    UnitData _unitData;

    public int UnitModel
    {
        get { return _unitModel; }
        set { _unitModel = value; }
    }

    public bool UnitCanClimbWalls
    {
        get { return _unitCanClimbWalls; }
        set { _unitCanClimbWalls = value; }
    }

    public int[] UnitCombatStats
    {
        get { return _unitCombatStats; }
        set { _unitCombatStats = value; }
    }

    public Vector3 UnitStartingWorldLoc
    {
        get { return _startingWorldLoc; }
        set { _startingWorldLoc = value; }
    }

    public CubeLocationScript CubeUnitIsOn
    {
        get { return _cubeUnitIsOn; }
        set { _cubeUnitIsOn = value; }
    }

    public int PlayerControllerID
    {
        get { return _playerControllerId; }
        set { _playerControllerId = value; }
    }

    public NetworkInstanceId NetID
    {
        get { return _netID; }
        set { _netID = value; }
    }

    public UnitData UnitData
    {
        get { return _unitData; }
        set { _unitData = value; }
    }

    public GameObject PlayerPivot
    {
        get { return _playerPivot; }
        set { _playerPivot = value; }
    }

    public Vector3 NodeID_UnitIsOn
    {
        get { return _nodeIDUnitIsOn; }
        set { _nodeIDUnitIsOn = value; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        _pathFindingNodes = new List<CubeLocationScript>();
        _rends = GetComponentsInChildren<Renderer>();
        PlayerPivot = transform.Find("Player_Pivot").gameObject;

        if (_pathFindingNodes == null) { Debug.LogError("We got a problem here"); }
        if (_rends == null) { Debug.LogError("We got a problem here"); }
        if (PlayerPivot == null) { Debug.LogError("We got a problem here"); }
    }

    // Use this for initialization
    void Start()
    {
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void PanelPieceChangeColor(string color) {

        if (_rends == null) {
            _rends = GetComponentsInChildren<Renderer>();
        }

        foreach (Renderer rend in _rends) {
			switch (color) {
			case "Red":
				rend.material.color = Color.red;
				break;
			case "Black":
				rend.material.color = Color.black;
				break;
			case "White":
				rend.material.color = Color.white;
				break;
			case "Green":
				rend.material.color = Color.green;
				break;
			default:
				break;
			}
		}
	}


    public void ActivateUnit()
    {
        PanelPieceChangeColor("Red");
        _unitActive = true;
    }


    public void DeActivateUnit()
    {
        PanelPieceChangeColor("White");
        _unitActive = false;
    }


    void OnMouseDown()
    {
        if (PlayerControllerID == PlayerManager.PlayerID)
        {
            if (!_unitActive)
            {
                ActivateUnit();
                UnitsManager.SetUnitActive(true, PlayerControllerID, (int)NetID.Value);
                UnitsManager.AssignCameraToActiveUnit();
            }
        }
	}

	void OnMouseOver() {
		if (!_unitActive)
        {
			PanelPieceChangeColor ("Green");
		}
	}
	void OnMouseExit() {
		if (!_unitActive)
        {
			PanelPieceChangeColor ("White");
		}
	}


    public void AssignPathFindingNodes(List<CubeLocationScript> nodes)
    {
        ClearPathFindingNodes();
        _pathFindingNodes = nodes;
        SetUnitToNextLocation_CLIENT();
    }

    public void ClearPathFindingNodes()
    {
        foreach(CubeLocationScript node in _pathFindingNodes)
        {
            node.DestroyPathFindingNode();
        }
        _pathFindingNodes.Clear();
        _pathNodeCount = 0;
    }

    public void SetUnitToNextLocation_CLIENT()
    {
        if (_pathNodeCount < _pathFindingNodes.Count)
        {
            Vector3 vect = _pathFindingNodes[_pathNodeCount].CubeStaticLocVector;
            CubeLocationScript script = LocationManager.GetLocationScript_CLIENT(vect);
            CubeUnitIsOn = script;
        }
        _pathNodeCount++;
    }

}
