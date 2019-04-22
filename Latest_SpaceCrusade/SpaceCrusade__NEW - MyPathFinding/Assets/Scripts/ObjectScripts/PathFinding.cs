using UnityEngine;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static PathFinding _instance;

    ////////////////////////////////////////////////

    public static bool _debugPathfindingNodes = false;

    ////////////////////////////////////////////////

    private static bool _unitCanClimbWalls = true;
    private static List<CubeLocationScript> _previousNodes = new List<CubeLocationScript>();

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

    public static List<Vector3> FindPath(UnitScript unit, Vector3 startVect, Vector3 targetVect) {

        Debug.Log("FindPath startVect: " + startVect);
        Debug.Log("FindPath trying to move to: " + targetVect);

        ResetPath();

        CubeLocationScript cubeStartScript = LocationManager.GetLocationScript(startVect);
		CubeLocationScript cubeTargetScript = LocationManager.GetLocationScript(targetVect);

        List<CubeLocationScript> openSet = new List<CubeLocationScript>();
		openSet.Clear ();
		HashSet<CubeLocationScript> closedSet = new HashSet<CubeLocationScript>();
		closedSet.Clear ();

		openSet.Add(cubeStartScript);
        _previousNodes.Add(cubeStartScript);

        while (openSet.Count > 0) {
			CubeLocationScript node = openSet [0];
			for (int i = 0; i < openSet.Count; i++) {
				if (openSet [i].fCost < node.fCost || openSet [i].fCost == node.fCost) {
					if (openSet [i].hCost < node.hCost)
						node = openSet [i];
				}
			}

			openSet.Remove (node);
			closedSet.Add (node);

			if (node == cubeTargetScript) { 
                return RetracePath (cubeStartScript, cubeTargetScript);
			}

            List<Vector3> neighVects = node.NeighbourVects;

            foreach (Vector3 vect in neighVects) {

                // personal checks
                if (LocationManager.CheckIfCanMoveToCube(unit, node, vect) == null)
                {
                    continue;
                }

                CubeLocationScript neightbourScript = LocationManager.GetLocationScript(vect);

                if (closedSet.Contains (neightbourScript)) {
                    continue;
				}

                if (_debugPathfindingNodes)
                {
                    neightbourScript.CreatePathFindingNode(unit.PlayerControllerID);
                }

                _previousNodes.Add(neightbourScript);

                int newCostToNeighbour = node.gCost + GetDistance (node, neightbourScript);
				if (newCostToNeighbour < neightbourScript.gCost || !openSet.Contains (neightbourScript)) {
					neightbourScript.gCost = newCostToNeighbour;
					neightbourScript.hCost = GetDistance (neightbourScript, cubeTargetScript);
					neightbourScript._parentPathFinding = node;

					if (!openSet.Contains (neightbourScript))
						openSet.Add (neightbourScript);
				}
			}
		} 
		Debug.Log ("SHIT NO WAY OF GETTING TO THAT SPOT!");
		return null;
	}

	private static List<Vector3> RetracePath(CubeLocationScript startNode, CubeLocationScript endNode) {
		List<Vector3> path = new List<Vector3>();
		CubeLocationScript currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode.CubeStaticLocVector);
			currentNode = currentNode._parentPathFinding;
		}
		path.Reverse();
        ResetPath();

        return path;
	}



	private static int GetDistance(CubeLocationScript nodeA, CubeLocationScript nodeB) {
		int dstX = (int)Mathf.Abs(nodeA.CubeStaticLocVector.x - nodeB.CubeStaticLocVector.x);
		int dstY = (int)Mathf.Abs(nodeA.CubeStaticLocVector.y - nodeB.CubeStaticLocVector.y);
        int dstZ = (int)Mathf.Abs(nodeA.CubeStaticLocVector.z - nodeB.CubeStaticLocVector.z);

        if (dstX > dstZ)
            //return 14* dstZ + 10* (dstX- dstZ);
            return 14 * dstZ + 10 * (dstX - dstZ) + 10 * dstY;
        //return 14*dstX + 10 * (dstZ - dstX);
        return 14 * dstX + 10 * (dstZ - dstX) + 10 * dstY;
    }

    private static void ResetPath()
    {
        foreach (CubeLocationScript node in _previousNodes)
        {
            //Debug.Log("pathfinding DestroyPathFindingNode: ");
            node.DestroyPathFindingNode();
        }
        _previousNodes.Clear();
    }
}