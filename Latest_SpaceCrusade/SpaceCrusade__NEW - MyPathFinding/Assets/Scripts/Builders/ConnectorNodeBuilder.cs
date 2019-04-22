using System.Collections.Generic;
using UnityEngine;

public class ConnectorNodeBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static ConnectorNodeBuilder _instance;

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

    // Get Connector Vects /////////////////////////////////////////////////////
    public static Dictionary<WorldNode, List<KeyValuePair<Vector3, int>>> GetConnectorVects(List<WorldNode> worldNodes)
    {
        Dictionary<WorldNode, List<KeyValuePair<Vector3, int>>> connectorVectsAndRotations = new Dictionary<WorldNode, List<KeyValuePair<Vector3, int>>>();

        // int floorBounds = MapSettings.worldSizeX * MapSettings.worldSizeZ; dont need this now for some weird reason
        int roofBounds = ((MapSettings.worldSizeX * MapSettings.worldSizeZ) * MapSettings.worldSizeY);

        foreach (WorldNode worldNode in worldNodes)
        {
            int nodeCount = worldNode.worldNodeCount;
            int[] worldNeighbours = worldNode.neighbours;
            List<KeyValuePair<Vector3, int>> vectList = new List<KeyValuePair<Vector3, int>>();

            if (worldNode.NodeSize == 1)
            {
                foreach (int worldNeigh in worldNeighbours)
                {
                    if (worldNeigh != -1) // out of bounds check
                    {
                        if (worldNeigh < roofBounds) // keeping nodes inside bounds
                        {
                            WorldNode neighbour = worldNodes[worldNeigh];
                            KeyValuePair<Vector3, int> vectorAndRot = GetVectsAndRotation(worldNode, neighbour, worldNeigh);
                            if (vectorAndRot.Value != -1) // weird issue still not sure why
                            {
                                vectList.Add(vectorAndRot);
                            }
                        }
                    }
                }
                connectorVectsAndRotations.Add(worldNode, vectList);
            }
        }
        return connectorVectsAndRotations;
    }

    public static KeyValuePair<Vector3, int> GetVectsAndRotation(WorldNode node0, WorldNode node1, int neighCount)
    {
        Vector3 connectionVect = new Vector3();

        bool initialSmaller = false;
        WorldNode smallerNode = null;
        WorldNode biggerNode = null;

        if (node0.NodeSize <= node1.NodeSize)
        {
            initialSmaller = true;
            smallerNode = node0;
            biggerNode = node1;
        }
        else if (node0.NodeSize > node1.NodeSize)
        {
            initialSmaller = false;
            smallerNode = node1;
            biggerNode = node0;
        }
        else
        {
            Debug.LogError("Something went wrong here");
        }


        int rotation = -1;

        Vector3 finalVect;
        Vector3 direction; // this is to seperate what axis x,y,z neighbour is

        if (initialSmaller)
        {
            direction = (biggerNode.NodeStaticLocation - smallerNode.NodeStaticLocation);
        }
        else
        {
            direction = (smallerNode.NodeStaticLocation - biggerNode.NodeStaticLocation);
        }


        finalVect = node0.NodeStaticLocation;

        if (direction.x != 0 && direction.y == 0 && direction.z == 0)
        {
            if (direction.x > 0)
            {
                rotation = 1;
                finalVect = new Vector3(finalVect.x + (MapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
            }
            else if (direction.x < 0)
            {
                rotation = 3;
                finalVect = new Vector3(finalVect.x - (MapSettings.sizeOfMapPiecesXZ), finalVect.y, finalVect.z);
            }
        }
        else if (direction.x == 0 && direction.y != 0 && direction.z == 0)
        {
            if (direction.y > 0)
            {
                rotation = 4; // these are the bastards making the connectors go UP
                finalVect = new Vector3(finalVect.x, finalVect.y + ((MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY)), finalVect.z);
            }
            else if (direction.y < 0)
            {
                rotation = 4;// these are the bastards making the connectors go UP
                finalVect = new Vector3(finalVect.x, finalVect.y - ((MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY)), finalVect.z);
            }
        }
        else if (direction.x == 0 && direction.y == 0 && direction.z != 0)
        {
            if (direction.z > 0)
            {
                rotation = 0;
                finalVect = new Vector3(finalVect.x, finalVect.y, finalVect.z + (MapSettings.sizeOfMapPiecesXZ));
            }
            else if (direction.z < 0)
            {
                rotation = 2;
                finalVect = new Vector3(finalVect.x, finalVect.y, finalVect.z - (MapSettings.sizeOfMapPiecesXZ));
            }
        }
        else
        {
            //Debug.LogError("SOMETHING WRONG HERE direction: " + direction);
            //Debug.LogFormat("initialSmaller: {0} -node0: {1} -node1: {2}", initialSmaller, node0.nodeLocation, node1.nodeLocation);
            return new KeyValuePair<Vector3, int>(new Vector3(-1, -1, -1), -1);
        }

        connectionVect = new Vector3(finalVect.x - 1, finalVect.y, finalVect.z - 1); // -1's to fix annoying postiioning issue

        return new KeyValuePair<Vector3, int>(connectionVect, rotation);
    }
    ////////////////////////////////////////////////////////////////////////////



    // Create Connector Nodes ////////////////////////////////////////////////
    public static Dictionary<WorldNode, List<ConnectorNode>> CreateConnectorNodes(Dictionary<WorldNode, List<KeyValuePair<Vector3, int>>> connectorVects)
    {
        // Wrap map Nodes around around Initial
        Dictionary<WorldNode, List<ConnectorNode>> worldNodeAndConnectorNodes = new Dictionary<WorldNode, List<ConnectorNode>>();

        foreach (WorldNode worldNode in connectorVects.Keys)
        {
            List<ConnectorNode> connectorNodes = new List<ConnectorNode>();
            List<KeyValuePair<Vector3, int>> vectsAndRot = connectorVects[worldNode];

            foreach (KeyValuePair<Vector3, int> pair in vectsAndRot)
            {
                Vector3 vector = pair.Key;
                int rotation = pair.Value;
                int mapType = MapSettings.MAPTYPE_CONNECT_FLOOR;
                int mapPiece = 0; // only 1 connector map piece atm for both vertical and horizontal pieces
                int nodeLayerCount = -1;

                ConnectorNode connectorNode = WorldBuilder._nodeBuilder.CreateNode<ConnectorNode>(worldNode.gameObject.transform, vector, rotation, mapType, mapPiece, NodeTypes.ConnectorNode);
                WorldBuilder._nodeBuilder.AttachCoverToNode(connectorNode, connectorNode.gameObject, CoverTypes.ConnectorCover, new Vector3(0, rotation * 90, 0));
                connectorNode.NodeSize = 1;
                connectorNode.neighbours = new int[6];
                for (int i = 0; i < connectorNode.neighbours.Length; i++)
                {
                    connectorNode.neighbours[i] = 1;
                }
                connectorNode.worldNodeParent = worldNode;

                connectorNodes.Add(connectorNode);

                // Up connector
                if (rotation == 4)
                {
                    connectorNode.connectorUp = true;
                    connectorNode.NodeMapType = MapSettings.MAPTYPE_CONNECT_UP_FLOOR;
                    connectorNode.transform.localEulerAngles = new Vector3(0, rotation * 90, 0);

                    Transform nodeCover = connectorNode.transform.Find("ConnectorCoverPrefab(Clone)");

                    nodeCover.localPosition = new Vector3(0, 0, 0); // to fix annoying vertical connector
                    nodeCover.localScale = new Vector3(8, 8, 8);
                    nodeCover.Find("Select").transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }


                if (worldNode.NodeSize == 1)
                {
                    nodeLayerCount = worldNode.NodeLayerCount + 4;  // 4 total layers in 1 map and vent piece
                }
                else
                {
                    Debug.LogError("something weird here");
                }
                connectorNode.NodeLayerCount = nodeLayerCount;

                LayerManager.AddNodeToLayer(connectorNode); // for camera layers

            }
            worldNode.connectorNodes = connectorNodes;
            worldNodeAndConnectorNodes.Add(worldNode, connectorNodes);


            ///// Connector Neighbours << This is a bit annoying is only using worldnode neighbours atm, might be cool to just use map neighbours instead... something to do
            int[] worldNodeNeighbours = worldNode.neighbours;

            if (worldNode.NodeSize == 1)
            {
                foreach (ConnectorNode connectorNode in worldNode.connectorNodes)
                {
                    int[] connectorNeighbours = connectorNode.neighbours;

                    for (int i = 0; i < worldNodeNeighbours.Length; i++)
                    {
                        if (worldNodeNeighbours[i] != -1)
                        {
                            connectorNeighbours[i] = 1;
                        }
                        else
                        {
                            connectorNeighbours[i] = -1;
                            //connectorNode.entranceSides.Add(i);
                        }
                    }
                }
            }
            ////////
        }
        return worldNodeAndConnectorNodes;
    }
    ////////////////////////////////////////////////////////////////////////////
}
