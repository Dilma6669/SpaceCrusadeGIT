using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkAgent : NetworkBehaviour
{
    /////////////////////////////////////////////////////

    Dictionary<NetworkInstanceId, GameObject> network_Client_Objects;
    Dictionary<Vector3, GameObject> network_Node_Containers;
    Dictionary<NetworkInstanceId, GameObject> network_Unit_Objects;

    /////////////////////////////////////////////////////
    /////////////////////////////////////////////////////

    // Use this for initialization
    void Awake()
    {
        network_Client_Objects = new Dictionary<NetworkInstanceId, GameObject>();
        network_Node_Containers = new Dictionary<Vector3, GameObject>();
        network_Unit_Objects = new Dictionary<NetworkInstanceId, GameObject>();
    }

    // Need this Start()
    void Start()
    {

        if (!isLocalPlayer) return;
        NetWorkManager.NetworkAgent = this;
    }

    /////////////////////////////////////////
    // HERLPER FUNCIONS:
    /////////////////////////////////////////

    private NetworkConnection TargetSpecificClient(NetworkInstanceId clientNetID)
    {
        return network_Client_Objects[clientNetID].GetComponent<NetworkIdentity>().connectionToClient;
    }


    /////////////////////////////////////////
    // TELL SERVER: CREATE NEW PLAYER OBJECT + ADD NEW PLAYER COUNT TO OTHER CLIENTS
    /////////////////////////////////////////

    [Command]
    public void CmdAddPlayerToSession(NetworkInstanceId clientID)
    {
        Start();
        network_Client_Objects.Add(clientID, ClientScene.FindLocalObject(clientID));

        SyncedVars _syncedVars = GameObject.Find("SyncedVars").GetComponent<SyncedVars>(); // needs to be here, function runs before awake
        if (_syncedVars == null) { Debug.LogError("We got a problem here"); }

        RpcUpdatePlayerCountOnClient(_syncedVars.PlayerCount + 1);
    }

    [ClientRpc]
    void RpcUpdatePlayerCountOnClient(int count)
    {
        GetComponent<PlayerAgent>().UpdatePlayerCount(count);
    }

    /////////////////////////////////////////
    // TELL SERVER: CREATE NETWORK NODE CONTAINER + ASSIGN LINK TO CLIENTS WORLD NODE
    /////////////////////////////////////////

    //[Command]
    //public void CmdTellServerToSpawnNetworkNodeContainer(NetworkInstanceId clientNetID, Vector3 nodeID)
    //{
    //    if (!isServer) return;

    //    GameObject parent = WorldManager._NetworkContainer;

    //    GameObject prefab = WorldBuilder._nodeBuilder.GetNetworkNodeContainerPrefab();
    //    GameObject networkNode = Instantiate(prefab, parent.transform, false);
    //    NetworkServer.Spawn(networkNode);

    //    network_Node_Containers.Add(nodeID, networkNode);
    //    LocationManager.SetNetworkNodeContainerScript_SERVER(nodeID, networkNode.GetComponent<NetworkNodeContainer>());

    //    networkNode.transform.SetParent(parent.transform); 
    //    networkNode.transform.position = nodeID;

    //    RpcLinkWorldNodeInClientsToNetworkNode(networkNode, nodeID);
    //}

    //[ClientRpc]
    //void RpcLinkWorldNodeInClientsToNetworkNode(GameObject networkNode, Vector3 nodeID)
    //{
    //    if (networkNode != null)
    //    {
    //        GameObject parent = WorldManager._NetworkContainer;
    //        NetworkNodeContainer script = networkNode.GetComponent<NetworkNodeContainer>();
    //        script.ContainerNodeID = nodeID;

    //        networkNode.transform.SetParent(parent.transform);

    //        GameObject worldNode = LocationManager.GetNodeLocationScript_CLIENT(nodeID).gameObject;
    //        worldNode.GetComponent<WorldNode>().NetworkNodeContainer = networkNode;
    //    }
    //}

    [Command]
    public void CmdTellServerToAssignUnitToNetworkNode(NetworkInstanceId unitNetID, Vector3 nodeID)
    {
        if (!isServer) return;
        GameObject parent = network_Node_Containers[nodeID];
        GameObject unit = network_Unit_Objects[unitNetID];

        unit.transform.SetParent(parent.transform);
    }


    /////////////////////////////////////////
    // TELL SERVER: MOVE NETWORK NODE + TELL CLIENT WORLD NODES TO FOLLOW
    /////////////////////////////////////////

    [Command]
    public void CmdTellServerToMoveWorldNode(Vector3 nodeID, Vector3 locPos, Vector3 locRot, float thrust)
    {
        if (!isServer) return;
        MovementManager.MoveNetworkNode_SERVER(nodeID, new KeyValuePair<Vector3, Vector3>(locPos, locRot));
        RpcTellAllClientsWorldNodeToFollowNetworkNode(nodeID, thrust);
    }

    [ClientRpc]
    void RpcTellAllClientsWorldNodeToFollowNetworkNode(Vector3 nodeID, float thrust)
    {
        GameObject worldNode = LocationManager.GetNodeLocationScript_CLIENT(nodeID).gameObject;
        worldNode.GetComponent<WorldNode>().MakeNodeFollowNetworkNode(thrust);
    }


    /////////////////////////////////////////
    // TELL SERVER: SPAWN SINGLE PLAYER UNIT + ASSIGN PROPERTIES TO CLIENTS COPY OF UNIT
    /////////////////////////////////////////

    [Command]
    public void CmdTellServerToSpawnPlayerUnit(NetworkInstanceId clientNetID, UnitData unitData, int playerID, Vector3 worldStart, Vector3 nodeID, bool lastUnit)
    {
        if (!isServer) return;

        unitData.UnitStartingWorldLoc = worldStart;

        GameObject parent = GameManager._UnitsManager;
        GameObject prefab = UnitsManager._unitBuilder.GetUnitModel(unitData.UnitModel);
        GameObject unit = Instantiate(prefab, parent.transform, false);
        NetworkServer.Spawn(unit);
        AssignUnitDataToUnitScript(unit, playerID, unitData, nodeID);

        if (unit != null)
        {
            network_Unit_Objects.Add(unit.GetComponent<NetworkIdentity>().netId, unit);
            unit.transform.position = unitData.UnitStartingWorldLoc;
            //CmdTellServerToAssignUnitToNetworkNode(unit.GetComponent<NetworkIdentity>().netId, nodeID);
            RpcUpdatePlayerUnitsOnAllClients(unit, playerID, unitData, nodeID);
        }
        else
        {
            Debug.LogError("Unit cannot be created on SERVER");
        }

        if(lastUnit)
        {
            TargetTellClientUnitsHaveAllLoaded(TargetSpecificClient(clientNetID));
        }
    }
    
    [ClientRpc]
    void RpcUpdatePlayerUnitsOnAllClients(GameObject unit, int playerID, UnitData unitData, Vector3 nodeID)
    {
        //if (isLocalPlayer)
        //{
            if (unit != null)
            {
                // units need to be assigned a parent for the layermanager + camera shit

                AssignUnitDataToUnitScript(unit, playerID, unitData, nodeID);

                GameObject parent = LocationManager.GetNodeLocationScript_CLIENT(nodeID).gameObject;
                unit.transform.SetParent(parent.transform);

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
        //}
    }
    [TargetRpc]
    public void TargetTellClientUnitsHaveAllLoaded(NetworkConnection target)
    {
        //if (isLocalPlayer)
        //{
            UnitsManager.AllPlayerUnitsHaveBeenLoaded();
       // }
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
        if (!isServer) return;
        Debug.Log("CmdTellServerToMoveUnit unitNetID.value " + (int)unitNetID.Value);

        GameObject unit = network_Unit_Objects[unitNetID];
        UnitScript unitScript = unit.GetComponent<UnitScript>();
        List<Vector3> pathVects = DataManipulation.ConvertIntArrayIntoVectors(movePath);

        unit.GetComponent<MovementScript>().MoveUnit(pathVects);
    }

    ////////////////////////////////////////////////

    // If the server works out before the client that a cube is inaccessable, then the server tells client to re-calculate path
    public void ServerTellClientToFindNewPathForUnit(NetworkInstanceId clientNetID, Vector3 finalTarget)
    {
        TargetTellClientToFindNewPathForUnit(TargetSpecificClient(clientNetID), finalTarget);
    }

    [TargetRpc]
    public void TargetTellClientToFindNewPathForUnit(NetworkConnection target, Vector3 finalTarget)
    {
        //if (isLocalPlayer)
        //{
            UnitsManager.MakeActiveUnitMove_CLIENT(finalTarget);
      //  }
    }

    ////////////////////////////////////////////////

    [Command] //The [Command] attribute indicates that the following function will be called by the Client, but will be run on the Server
    public void CmdTellServerToUpdateLocation(Vector3 vect, bool[] cubeData)
    {
        if (!isServer) return;
        LocationManager.UpdateServerLocation_SERVER(vect, cubeData);
    }

}
