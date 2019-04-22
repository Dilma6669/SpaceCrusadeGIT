using System.Collections.Generic;
using UnityEngine;

public class WorldBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static WorldBuilder _instance;

    ////////////////////////////////////////////////

    public static NodeBuilder _nodeBuilder;

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
        _nodeBuilder = GameObject.Find("NodeBuilder").GetComponent<NodeBuilder>();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////


    public static void BuildWorldNodes()
    {
        List<Vector3> worldVects;
        Dictionary<WorldNode, List<MapNode>> worldAndWrapperNodes;
        Dictionary<WorldNode, List<ConnectorNode>> worldAndconnectorNodes;
        Dictionary<WorldNode, List<KeyValuePair<Vector3, int>>> connectorVectsAndRotations;

        List<List<Vector3>> container = WorldNodeBuilder.GetWorld_Outer_DockingVects();
        worldVects = container[0];

        WorldNodeBuilder.CreateWorldNodes(worldVects);
        List<WorldNode> worldNodes = WorldNodeBuilder.GetWorldNodes();
        WorldNodeBuilder.GetWorldNodeNeighbours();
        MapNodeBuilder.CreateMapNodes(worldNodes);

        connectorVectsAndRotations = ConnectorNodeBuilder.GetConnectorVects(worldNodes);
        worldAndconnectorNodes = ConnectorNodeBuilder.CreateConnectorNodes(connectorVectsAndRotations);
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public static void AttachMapToNode<T>(T node) where T : BaseNode
    {
        if (node.NodeType == NodeTypes.WorldNode)
        {
            WorldNode worldNode = node as WorldNode;
            AttachMapPiecesToWorldNode(worldNode);
        }

        if (node.NodeType == NodeTypes.MapNode)
        {
            MapNode mapNode = node as MapNode;
            AttachMapPieceToMapNode(mapNode);
        }

        if (node.NodeType == NodeTypes.ConnectorNode)
        {
            ConnectorNode connectNode = node as ConnectorNode;
            AttachMapPieceToConnectorNode(connectNode);
        }
       // Debug.Log("AttachMapToNode FINISHED");
    }

    ////////////////////////////////////////////////

    private static void AttachMapPiecesToWorldNode(WorldNode worldNode)
    {
        int mapCount = 0;
        foreach (MapNode mapNode in worldNode.mapNodes)
        {
            mapNode.entrance = true;
            Vector3 nodeVect = mapNode.NodeStaticLocation;
            int mapSize = mapNode.NodeSize;
            int rotation = mapNode.NodeStaticRotation;
            int mapType = mapNode.NodeMapType;
            int mapPiece = mapCount;
            // for the players small ship
            if (mapCount == 4)
            {
                mapNode.playerShipMapPART1 = true;
            }
            if (mapCount == 13)
            {
                mapNode.playerShipMapPART2 = true;
            }
            GridBuilder.BuildLocationGrid(mapNode, mapSize);
            List<Vector3> mapPieceNodes = GridBuilder.GetGridNodeStartPositions();
            MapPieceBuilder.SetWorldNodeNeighboursForDock(worldNode.neighbours); // for the ship docks
            MapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, mapSize, mapType, mapPiece, rotation);
            MapPieceBuilder.SetPanelsNeighbours();
            //mapNode.mapFloorData = MapPieceBuilder.MapFloorData;
            //mapNode.mapVentData = MapPieceBuilder.MapVentData;
            mapCount++;
        }
        GameManager.StartGame(worldNode.NodeStaticLocation); // <<<<<< start the fucking game bitch
    }


    private static void AttachMapPieceToMapNode(MapNode mapNode)
    {
        Vector3 nodeVect = mapNode.NodeStaticLocation;
        int mapSize = mapNode.NodeSize;
        int rotation = mapNode.NodeStaticRotation;
        int mapType = mapNode.NodeMapType;
        int mapPiece = mapNode.NodeMapPiece;

        GridBuilder.BuildLocationGrid(mapNode, mapSize);
        List<Vector3> mapPieceNodes = GridBuilder.GetGridNodeStartPositions();
        MapPieceBuilder.AttachMapPieceToMapNode(mapNode, mapPieceNodes, mapSize, mapType, mapPiece, rotation);
        MapPieceBuilder.SetPanelsNeighbours();
        //mapNode.RemoveDoorPanels();
        //mapNode.mapFloorData = MapPieceBuilder.MapFloorData;
        //mapNode.mapVentData = MapPieceBuilder.MapVentData;
    }


    private static void AttachMapPieceToConnectorNode(ConnectorNode connectNode)
    {
        Vector3 nodeVect = connectNode.NodeStaticLocation;
        int mapSize = connectNode.NodeSize;
        int rotation = connectNode.NodeStaticRotation;
        int mapType = connectNode.NodeMapType;
        int mapPiece = connectNode.NodeMapPiece;

        GridBuilder.BuildLocationGrid(connectNode, mapSize);
        List<Vector3> mapPieceNodes = GridBuilder.GetGridNodeStartPositions();
        MapPieceBuilder.AttachMapPieceToMapNode(connectNode, mapPieceNodes, mapSize, mapType, mapPiece, rotation);
        MapPieceBuilder.SetPanelsNeighbours();
        //connectNode.mapFloorData = _mapPieceBuilder.GetMapFloorData();
        //connectNode.mapVentData = _mapPieceBuilder.GetMapVentData();
    }

}
