using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    /////////////////////////////////////////////////////

    Dictionary<NetworkInstanceId, GameObject> network_Client_Objects;
    Dictionary<NetworkInstanceId, GameObject> network_Unit_Objects;

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////

    // Use this for initialization
    void Awake()
    {
        network_Client_Objects = new Dictionary<NetworkInstanceId, GameObject>();
        network_Unit_Objects = new Dictionary<NetworkInstanceId, GameObject>();
    }

    // Need this Start()
    void Start()
    {

        if (!isLocalPlayer) return;
        NetWorkManager.NetworkAgent = this;
    }

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////

    [Command] //Commands - which are called from the client and run on the server;
    public void CmdAddPlayerToSession(NetworkInstanceId clientID)
    {
        Start();
        network_Client_Objects.Add(clientID, ClientScene.FindLocalObject(clientID));

        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake
        if (_syncedVars == null) { Debug.LogError("We got a problem here"); }

        RpcUpdatePlayerCountOnClient(_syncedVars.PlayerCount + 1);
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcUpdatePlayerCountOnClient(int count)
    {
        GetComponent<PlayerAgent>().UpdatePlayerCount(count);
    }

    ////////////////////////////////////////////////

    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToSpawnPlayerUnit(NetworkInstanceId clientNetID, UnitData unitData, int playerID, Vector3 worldStart, Vector3 nodeID, bool lastUnit)
    {
        //Debug.Log("CmdTellServerToSpawnPlayerUnit ");
        if (!isServer) return;

        unitData.UnitStartingWorldLoc = worldStart;

        GameObject parent = LocationManager.GetNodeLocationScript_SERVER(nodeID).gameObject;

        GameObject prefab = UnitsManager._unitBuilder.GetUnitModel(unitData.UnitModel);
        GameObject unit = Instantiate(prefab, parent.transform, false);
        NetworkServer.Spawn(unit);
        AssignUnitDataToUnitScript(unit, playerID, unitData, nodeID);

        unit.transform.SetParent(parent.transform); // <<, important to put unit INSIDE current world Node, + needs to change when they move to other world node (TO DO!!)

        if (unit != null)
        {
            network_Unit_Objects.Add(unit.GetComponent<NetworkIdentity>().netId, unit);
            unit.transform.position = unitData.UnitStartingWorldLoc;
            UnitScript unitScript = unit.GetComponent<UnitScript>();
            //LocationManager.SetUnitOnCube_SERVER(unitScript, unitScript.UnitStartingWorldLoc);
            //NetworkInstanceId unitNetID = unit.GetComponent<NetworkIdentity>().netId;
            RpcUpdatePlayerUnitsOnAllClients(unit, playerID, unitData, nodeID);
        }
        else
        {
            Debug.LogError("Unit cannot be created on SERVER");
        }

        if(lastUnit)
        {
            NetworkConnection clientID = network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
            TargetTellClientUnitsHaveAllLoaded(clientID);
        }
    }
    [ClientRpc] //ClientRpc calls - which are called on the server and run on clients
    void RpcUpdatePlayerUnitsOnAllClients(GameObject unit, int playerID, UnitData unitData, Vector3 nodeID)
    {
        if (isLocalPlayer)
        {
            if (unit != null)
            {
                // units dont need to be assigned to a parent on the client <<<<<< just a note to future self

                AssignUnitDataToUnitScript(unit, playerID, unitData, nodeID);

                UnitScript unitScript = unit.GetComponent<UnitScript>();
                Debug.Log("Unit Succesfully created on CLIENT: " + (int)unitScript.NetID.Value);
                UnitsManager.AddUnitToGame(playerID, (int)unitScript.NetID.Value, unitScript); // add unit to generic unit manager pool

                LocationManager.SetUnitOnCube_CLIENT(unit.GetComponent<UnitScript>(), unitScript.UnitStartingWorldLoc);

                if (playerID == PlayerManager.PlayerID)
                {
                    GetComponent<UnitsAgent>().AddUnitToUnitAgent(unitScript); // add unit to a more specific player unit pool

                    if (unitData.UnitCombatStats[0] == 1) // if rank is 'Captain'???? then make active
                    {
                        UnitsManager.SetUnitActive(true, playerID, (int)unitScript.NetID.Value);
                    }
                }
            }
            else
            {
                Debug.Log("Unit cannot be created on CLIENT");
            }
        }
    }
    [TargetRpc]
    public void TargetTellClientUnitsHaveAllLoaded(NetworkConnection target)
    {
        UnitsManager.AllPlayerUnitsHaveBeenLoaded();
    }


    ////////////////////////////////////////////////

    void AssignUnitDataToUnitScript(GameObject unit, int playerID, UnitData unitData, Vector3 nodeID)
    {
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        unitScript.UnitData = unitData;
        unitScript.NetID = unit.GetComponent<NetworkIdentity>().netId;
        unitScript.PlayerControllerID = playerID;
        unitScript.UnitModel = unitData.UnitModel;
        unitScript.UnitCanClimbWalls = unitData.UnitCanClimbWalls;
        unitScript.UnitStartingWorldLoc = unitData.UnitStartingWorldLoc;
        unitScript.UnitCombatStats = unitData.UnitCombatStats;
        unitScript.NodeID_UnitIsOn = nodeID;
    }

    ////////////////////////////////////////////////

    // Server Move Unit 
    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToMoveUnit(NetworkInstanceId clientNetID, NetworkInstanceId unitNetID, int[] movePath)
    {
        Debug.Log("CmdTellServerToMoveUnit unitNetID.value " + (int)unitNetID.Value);
        if (!isServer) return;
        
        GameObject unit = network_Unit_Objects[unitNetID];
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        List<Vector3> pathVects = DataManipulation.ConvertIntArrayIntoVectors(movePath);

        unit.GetComponent<MovementScript>().MoveUnit(pathVects);
    }

    ////////////////////////////////////////////////

    // If the server works out before the client that a cube is inaccessable, then the server tells client to re-calculate path
    public void ServerTellClientToFindNewPathForUnit(NetworkInstanceId clientNetID, Vector3 finalTarget)
    {
        NetworkConnection clientID = network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
        TargetTellClientToFindNewPathForUnit(clientID, finalTarget);
    }

    [TargetRpc]
    public void TargetTellClientToFindNewPathForUnit(NetworkConnection target, Vector3 finalTarget)
    {
        UnitsManager.MakeActiveUnitMove_CLIENT(finalTarget);
    }

    ////////////////////////////////////////////////

    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToUpdateLocation(Vector3 vect, bool[] cubeData)
    {
        LocationManager.UpdateServerLocation_SERVER(vect, cubeData);
    }



    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToMoveWorldNode(Vector3 nodeID, Vector3 locPos, Vector3 locRot)
    {
        if (!isServer) return;
        MovementManager.MoveMapNode_SERVER(nodeID, new KeyValuePair<Vector3, Vector3>(locPos, locRot));
    }

}
