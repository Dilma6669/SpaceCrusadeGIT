using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class UnitsAgent : NetworkBehaviour {

    /*
    GameManager _gameManager;

    NetworkManager _networkManager;

    PlayerManager _playerManager;


    UnitScript _activeUnit = null;

    public List<GameObject> unitObjects = new List<GameObject>();
    public List<UnitScript> unitScripts = new List<UnitScript>();

    // Use this for initialization
    void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _playerManager = _gameManager._playerManager;
        if (_playerManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }



    public void SetUnitActive(bool onOff, UnitScript unit = null)
    {
        if (onOff)
        {
            if (_activeUnit)
            {
                _activeUnit.ActivateUnit(false); 
            }
            //_gameManager._cubeManager.SetCubeActive (false);
            _activeUnit = unit;
            //_gameManager._locationManager.DebugTestPathFindingNodes(_activeUnit);
        }
        else
        {
            _activeUnit = null;
        }
    }

    public void MakeActiveUnitMove(Vector3 vectorToMoveTo)
    {
        if (_activeUnit)
        {
            NetworkInstanceId clientNetID = GetComponent<PlayerAgent>().NetID;
            NetworkInstanceId unitNetID = _activeUnit.NetID;
            GetComponent<NetworkAgent>().CmdTellServerToMoveUnit(clientNetID, unitNetID, vectorToMoveTo);
        }
    }

    public void MakeUnitRecalculateMove(UnitScript unit, Vector3 vectorToMoveTo)
    {
        Debug.Log("recalulating from unitsAgent");
        NetworkInstanceId clientNetID = GetComponent<PlayerAgent>().NetID;
        NetworkInstanceId unitNetID = unit.NetID;
        GetComponent<NetworkAgent>().CmdTellServerToMoveUnit(clientNetID, unitNetID, vectorToMoveTo);
    }


    public void SetUpUnitForPlayer(GameObject unit)
    {
        if (isLocalPlayer)
        {
            Debug.Log("fucken unit 3: " + unit);
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            unitScript.CubeUnitIsOn = _gameManager._locationManager.GetLocationScript(unitScript.UnitStartingWorldLoc);
            unitScript.PlayerControllerID = _playerManager.PlayerID;
            Debug.Log("fucken unitScript.PlayerControllerID 1: " + unitScript.PlayerControllerID);
        }
    }
    */


}
