using System.Collections.Generic;
using UnityEngine;

public class MapNodeBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static MapNodeBuilder _instance;

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

    // Get Map Vects ////////////////////////////////////////////////////////////
    private static List<Vector3> GetMapVects(WorldNode nodeScript)
    {
        Vector3 loc = nodeScript.NodeStaticLocation;
        int size = nodeScript.NodeSize;

        List<Vector3> nodeVects = new List<Vector3>();

        int multiplierXZ = (int)Mathf.Floor(size / 2) * MapSettings.sizeOfMapPiecesXZ;
        int multiplierY = (int)Mathf.Floor(size / 2) * (MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY);

        int countY = (int)loc.y - multiplierY;
        int countZ = (int)loc.z - multiplierXZ;
        int countX = (int)loc.x - multiplierXZ;

        for (int y = 0; y < size; y++)
        {
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Debug.Log("Vector3 (gridLoc): x: " + x + " y: " + y + " z: " + z);
                    nodeVects.Add(new Vector3(countX - 1, countY, countZ - 1)); // -1's to fix annoying postiioning issue
                    countX += MapSettings.sizeOfMapPiecesXZ;
                }
                countX = (int)loc.x - multiplierXZ;
                countZ += MapSettings.sizeOfMapPiecesXZ;
            }
            countX = (int)loc.x - multiplierXZ;
            countZ = (int)loc.z - multiplierXZ;
            countY += (MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY);
        }

        return nodeVects;
    }
    ////////////////////////////////////////////////////////////////////////////


    // Create Map Nodes ////////////////////////////////////////////////////////
    public static Dictionary<WorldNode, List<MapNode>> CreateMapNodes(List<WorldNode> worldNodes)
    {
        // Wrap map Nodes around around Initial
        Dictionary<WorldNode, List<MapNode>> worldNodeAndWrapperNodes = new Dictionary<WorldNode, List<MapNode>>();

        foreach (WorldNode worldNode in worldNodes)
        {
            List<Vector3> mapVects = GetMapVects(worldNode);
            List<MapNode> mapNodes = new List<MapNode>();

            int mapCount = 1;
            int layerCount = 0;
            int nodeLayerCount = -1;
            foreach (Vector3 vect in mapVects)
            {
                int rotation = 0;
                int mapType = -1;
                if (worldNode.entrance)
                {
                    mapType = MapSettings.MAPTYPE_SHIPPORT_FLOOR;
                }
                else
                {
                    mapType = MapSettings.MAPTYPE_MAP_FLOOR;
                }
                int mapPiece = Random.Range(0, 4); // 3 is the number of availble map pieces

                MapNode mapNode = WorldBuilder._nodeBuilder.CreateNode<MapNode>(worldNode.gameObject.transform, vect, rotation, mapType, mapPiece, NodeTypes.MapNode);
                mapNode.NodeSize = 1;
                mapNode.neighbours = new int[6];
                for (int i = 0; i < mapNode.neighbours.Length; i++)
                {
                    mapNode.neighbours[i] = 1;
                }
                mapNode.worldNodeParent = worldNode;


                if (worldNode.NodeSize == 1)
                {
                    nodeLayerCount = worldNode.NodeLayerCount + 4; // 4 total layers in 1 map and vent piece
                }
                else if (worldNode.NodeSize == 3)
                {
                    nodeLayerCount = worldNode.NodeLayerCount + layerCount;

                    if (mapCount % 9 == 0)
                    {
                        layerCount = layerCount + 4;  // 4 total layers in 1 map and vent piece
                    }
                }
                else
                {
                    Debug.LogError("something weird here");
                }
                mapNode.NodeLayerCount = nodeLayerCount;


                mapNodes.Add(mapNode);
                if (!worldNode.entrance)
                {
                    WorldBuilder._nodeBuilder.AttachCoverToNode(mapNode, mapNode.gameObject, CoverTypes.NormalCover, new Vector3(0,0,0));
                }

                LayerManager.AddNodeToLayer(mapNode); // for camera layers

                mapCount++;
            }
            worldNode.mapNodes = mapNodes;
            worldNodeAndWrapperNodes.Add(worldNode, mapNodes);

            ////////////////

            //// Map Neighbours
            int[] worldNodeNeighbours = worldNode.neighbours;

            if (worldNode.NodeSize == 1)
            {
                MapNode mapNode = worldNode.mapNodes[0];
                int[] mapNeighbours = mapNode.neighbours;

                for (int i = 0; i < worldNodeNeighbours.Length; i++)
                {
                    if (worldNodeNeighbours[i] != -1)
                    {
                        mapNeighbours[i] = 1;
                    }
                    else
                    {
                        mapNeighbours[i] = -1;
                        //mapNode.entranceSides.Add(i);
                    }
                }
            }
            ////////
            if (worldNode.NodeSize == 3)
            {
                // bottom
                SetMapNeighboursWithMultipleLinks(worldNode, 0, 4, new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }, worldNode.entrance);
                // Front
                SetMapNeighboursWithMultipleLinks(worldNode, 1, 10, new int[] { 0, 1, 2, 9, 10, 11, 18, 19, 20 }, worldNode.entrance);
                // Left
                SetMapNeighboursWithMultipleLinks(worldNode, 2, 12, new int[] { 0, 3, 6, 9, 12, 15, 18, 21, 24 }, worldNode.entrance);
                // Right
                SetMapNeighboursWithMultipleLinks(worldNode, 3, 14, new int[] { 2, 5, 8, 11, 14, 17, 20, 23, 26 }, worldNode.entrance);
                // Back
                SetMapNeighboursWithMultipleLinks(worldNode, 4, 16, new int[] { 6, 7, 8, 15, 16, 17, 24, 25, 26 }, worldNode.entrance);
                // Top
                SetMapNeighboursWithMultipleLinks(worldNode, 5, 22, new int[] { 18, 19, 20, 21, 22, 23, 24, 25, 26 }, worldNode.entrance);
            }
        }

        return worldNodeAndWrapperNodes;
    }
    ////////////////////////////////////////////////////////////////////////////

    // SetUp MapNode Connections to neighbours ////////////////////////////////////////////////////////
    private static void SetMapNeighboursWithMultipleLinks(WorldNode worldNode, int worldNeighCount, int singleLinkCount, int[] multipleLinkCounts, bool shipEntrance)
    {
        int[] worldNodeNeighbours = worldNode.neighbours;

        if (worldNodeNeighbours[worldNeighCount] != -1)
        {
            WorldNode worldNeighbour = WorldNodeBuilder.GetWorldNode(worldNodeNeighbours[worldNeighCount]);

            if (worldNeighbour.NodeSize == 1)
            {
                foreach (int link in multipleLinkCounts)
                {
                    worldNode.mapNodes[link].neighbours[worldNeighCount] = -1;
                }
                worldNode.mapNodes[singleLinkCount].neighbours[worldNeighCount] = 1; // for the middle front connector
            }
            if (worldNeighbour.NodeSize == 3)
            {
                foreach (int link in multipleLinkCounts)
                {
                    worldNode.mapNodes[link].neighbours[worldNeighCount] = 1;
                }
            }
        }
        else
        {
            foreach (int link in multipleLinkCounts)
            {
                worldNode.mapNodes[link].neighbours[worldNeighCount] = -1;
                //worldNode.mapNodes[link].entranceSides.Add(worldNeighCount);
            }
        }
    }
    ////////////////////////////////////////////////////////////////////////////
}
