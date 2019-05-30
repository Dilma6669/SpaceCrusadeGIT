using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static MovementManager _instance;

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

    public static List<Vector3> SetUnitsPath(UnitScript unitScript, Vector3 start, Vector3 end)
    {
        List<Vector3> path = PathFinding.FindPath(unitScript, start, end);

        if (path == null) { print("path == null");  return null; }

        print("SetUnitsPath path start >> " + start + " >>>>>> " + path[path.Count - 1]);

        foreach (Vector3 vect in path)
        {
            LocationManager.SendCubeDataToSERVER_CLIENT(vect);
        }
        return path;
    }


    public static void StopUnits() {


	}

    public static void CreatePathFindingNodes_CLIENT(UnitScript unitScript, int unitNetID, List<Vector3> path)
    {
        unitScript.ClearPathFindingNodes();

        List<CubeLocationScript> scriptList = new List<CubeLocationScript>();

        foreach(Vector3 vect in path)
        {
            CubeLocationScript script = LocationManager.GetLocationScript_CLIENT(vect);
            script.CreatePathFindingNodeInCube(unitNetID);
            scriptList.Add(script);
            Debug.Log("pathfinding VISUAL node set at vect: " + vect);
        }

        unitScript.AssignPathFindingNodes(scriptList);
    }

    ////////////////////////////////////////////////

    public static void MoveNetworkNode_SERVER(Vector3 nodeID, KeyValuePair<Vector3, Vector3> posRot)
    {
        Debug.Log("Moving Map node nodeID: " + nodeID);
        NetworkNodeContainer nodeScript = LocationManager.GetNetworkNodeContainerScript_SERVER(nodeID);
        Debug.Log("Moving Map node nodeScript: " + nodeScript);

        nodeScript.MoveNetworkNode(posRot);
    }
}
