using System.Collections.Generic;
using UnityEngine;

public class WorldNodeBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static WorldNodeBuilder _instance;

    ////////////////////////////////////////////////

    private static List<WorldNode> _WorldNodes;

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

    public static WorldNode GetWorldNode(int index)
    {
        return _WorldNodes[index];
    }

    public static List<WorldNode> GetWorldNodes()
    {
        return _WorldNodes;
    }

    // Get World Outer Docking Vects ////////////////////////////////////////////
    public static List<List<Vector3>> GetWorld_Outer_DockingVects()
    {
        List<List<Vector3>> container = new List<List<Vector3>>();

        List<Vector3> worldVects = new List<Vector3>();
        List<Vector3> outerVects = new List<Vector3>();
        List<Vector3> dockingVects = new List<Vector3>();

        int countY = MapSettings.worldPadding; // First node position
        int countZ = MapSettings.worldPadding;
        int countX = MapSettings.worldPadding;

        int nodeDistanceXZ = MapSettings.worldNodeDistanceXZ + 1; // + 1 cause distance is only space IN-between nodes
        int nodeDistanceY = MapSettings.worldNodeDistanceY + 1;

        int worldSizeY = MapSettings.worldSizeY + 2; // +2 for Outer/Ship Zones
        int worldSizeZ = MapSettings.worldSizeZ + 2; // +2 for Outer/Ship Zones
        int worldSizeX = MapSettings.worldSizeX + 2; // +2 for Outer/Ship Zones

        //int centralOuterNodeX = 2;
        //int dockingNodeX = 3;

        for (int y = 0; y < worldSizeY; y++)
        {
            for (int z = 0; z < worldSizeZ; z++)
            {
                for (int x = 0; x < worldSizeX; x++)
                {
                    //Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    int resultY = countY * (MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY);
                    int resultZ = countZ * MapSettings.sizeOfMapPiecesXZ;
                    int resultX = countX * MapSettings.sizeOfMapPiecesXZ;

                    if ((x == 0) || (z == 0) || (x == (worldSizeX - 1)) || (z == (worldSizeZ - 1))
                        || y == 0 || y == worldSizeY - 1)
                    {
                        // Get outer Zone central node
                        // if (z == (worldSizeX - 2) && x == centralOuterNodeX && y == 0)
                        //  {
                        outerVects.Add(new Vector3(resultX, resultY, resultZ));
                        //  } 

                        // Get docking lines
                        /* if (z == 0 && x == dockingNodeX)
                         {
                             dockingVects.Add(new Vector3(resultX, resultY, resultZ));
                         }*/
                    }
                    else // All central map nodes
                    {
                        worldVects.Add(new Vector3(resultX, resultY, resultZ));
                    }


                    countX += nodeDistanceXZ;
                }
                countX = MapSettings.worldPadding;
                countZ += nodeDistanceXZ;
            }
            countX = MapSettings.worldPadding;
            countZ = MapSettings.worldPadding;
            countY += nodeDistanceY;
        }

        container.Add(worldVects);
        container.Add(outerVects);
        container.Add(dockingVects);

        return container;
    }
    ////////////////////////////////////////////////////////////////////////////


    // Create World Nodes ///////////////////////////////////////////////////
    public static void CreateWorldNodes(List<Vector3> nodeVects)
    {
        // build inital map Node
        _WorldNodes = new List<WorldNode>();

        int rowMultipler = MapSettings.worldSizeX;
        int colMultiplier = MapSettings.worldSizeZ;

        int totalMultiplier = MapSettings.worldSizeX * MapSettings.worldSizeZ;

        WorldNodeData_01 worldFloor = ScriptableObject.CreateInstance<WorldNodeData_01>();
        List<int[,]> floors = worldFloor.floors;

        int countFloorX = 0; // complicated to explain to future me
        int countFloorZ = 0;
        int countFloorY = 1;

        int count = 1;
        foreach (Vector3 vect in nodeVects)
        {
            int rotation = 0;
            int mapType = -1;
            int mapPiece = -1;

            WorldNode nodeScript = WorldBuilder._nodeBuilder.CreateNode<WorldNode>(WorldManager._WorldContainer.transform, vect, rotation, mapType, mapPiece, NodeTypes.WorldNode);
            nodeScript.worldNodeCount = (count - 1);
            nodeScript.NodeLayerCount = countFloorY * 12; // 12 is the number of (mapieces + vents layers) in the y axis of a worldnode

            // for the specified map structures
            if (MapSettings.LOADPREBUILT_STRUCTURE)
            {
                int[,] floor = floors[countFloorY - 1];
                if (floor[countFloorX, countFloorZ] == 01)
                {
                    int randSize = MapSettings.getRandomMapSize;
                    nodeScript.NodeSize = randSize;
                }
                else
                {
                    nodeScript.NodeSize = 0;
                }
            }
            else
            {
                int randSize = MapSettings.getRandomMapSize;
                nodeScript.NodeSize = randSize;
            }


            int shipEntranceProbablity = 20;

            if (nodeScript.NodeSize == 3 && Random.Range(0, shipEntranceProbablity) == 0)
            {
                WorldBuilder._nodeBuilder.AttachCoverToNode(nodeScript, nodeScript.gameObject, CoverTypes.LargeGarageCover, new Vector3(0, 0, 0));
                nodeScript.entrance = true;
            }


            _WorldNodes.Add(nodeScript);

            // for counting, best not to change, even tho its ugly
            countFloorX++;
            if (count % totalMultiplier == 0)
            {
                countFloorY++;
            }
            if (countFloorX % rowMultipler == 0)
            {
                countFloorX = 0;
                countFloorZ++;
            }
            if (countFloorZ % colMultiplier == 0)
            {
                countFloorZ = 0;
            }

            //  think need to add world nodes to layer sytem
            LayerManager.AddNodeToLayer(nodeScript); // for camera layers

            // for the dynamic grid experiment
            LocationManager.SetNodeScriptToLocation(vect, nodeScript);

            count++;
        }
    }
    ////////////////////////////////////////////////////////////////////////////

    // Get World Node Neighbours ///////////////////////////////////////////////
    public static void GetWorldNodeNeighbours()
    {
        // build inital map Node
        List<WorldNode> worldNodes = new List<WorldNode>();
        bool left = true;
        bool right = false;
        bool front = false;
        bool back = true;

        int rowMultipler = MapSettings.worldSizeX;
        int colMultiplier = MapSettings.worldSizeZ;

        int totalMultiplier = MapSettings.worldSizeX * MapSettings.worldSizeZ;

        int countFloorY = 1;

        int count = 1;
        foreach (WorldNode worldNode in _WorldNodes)
        {

            //Debug.Log("worldNode.worldNodeCount: " + worldNode.worldNodeCount);
            // for neighbours
            right = (count % rowMultipler == 0) ? true : false;
            left = (count == 1 || ((count - 1) % rowMultipler == 0)) ? true : false;
            front = ((count + MapSettings.worldSizeX) > (totalMultiplier * countFloorY) && count <= (totalMultiplier * countFloorY)) ? true : false;
            back = (count >= ((totalMultiplier + 1) * (countFloorY - 1)) && count <= (totalMultiplier * (countFloorY - 1)) + MapSettings.worldSizeX) ? true : false;

            worldNode.neighbours = GetNeighbours((count - 1), left, right, front, back);

            // for counting, best not to change, even tho its ugly
            if (count % totalMultiplier == 0)
            {
                countFloorY++;
            }
            count++;
        }
    }



    private static int[] GetNeighbours(int count, bool left, bool right, bool front, bool back)
    {
        int[] neighbours = new int[6];

        neighbours[0] = count - (MapSettings.worldSizeX * MapSettings.worldSizeZ);//(x, y - 1, z)
        neighbours[1] = (back) ? -1 : count - MapSettings.worldSizeX;              //(x, y, z - 1)
        neighbours[2] = (left) ? -1 : count - 1;                                    //(x - 1, y, z)
        neighbours[3] = (right) ? -1 : count + 1;                                   //(x + 1, y, z)
        neighbours[4] = (front) ? -1 : count + MapSettings.worldSizeX;             //(x, y, z + 1)
        neighbours[5] = count + (MapSettings.worldSizeX * MapSettings.worldSizeZ);//(x, y + 1, z)

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (neighbours[i] <= -1)
            {
                neighbours[i] = -1;
            }
            else if (neighbours[i] >= _WorldNodes.Count)
            {
                neighbours[i] = -1;
            }
            else if (_WorldNodes[neighbours[i]].NodeSize < 1)
            {
                neighbours[i] = -1;
            }
        }
        return neighbours;
    }
    ////////////////////////////////////////////////////////////////////////////
}
