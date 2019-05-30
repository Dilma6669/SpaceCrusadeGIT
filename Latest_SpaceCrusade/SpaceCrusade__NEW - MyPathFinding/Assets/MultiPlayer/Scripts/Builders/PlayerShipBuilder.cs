using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipBuilder : MonoBehaviour {

    LocationManager LocationManager;
    CubeBuilder _cubeBuilder;
    MapSettings MapSettings;


    public List<int[,]> floors = new List<int[,]>();
    public List<int[,]> vents = new List<int[,]>();


    void Awake()
    {
        LocationManager = transform.parent.GetComponent<LocationManager>();
        if (LocationManager == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        _cubeBuilder = transform.parent.GetComponentInChildren<CubeBuilder>();
        if (_cubeBuilder == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        MapSettings = transform.parent.GetComponent<MapSettings>();
        if (MapSettings == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }


    public void AttachShipPieceToMapNode(List<Vector3> nodes)
    {
        StartCoroutine(BuildShipPiecesByIEnum(nodes, 0.001f));
    }


    private IEnumerator BuildShipPiecesByIEnum(List<Vector3> nodes, float waitTime)
    {
        /*
        Vector3 GridLoc;

        List<int[,]> layers = new List<int[,]>();
        int[,] floor;

        int sizeSquared = (MapSettings.numMapPiecesXZ * 4) + 4; // calculating area count for area AROUND mainShip
        int nodeCount = 0;
        int layerCount = -1;


        for (int j = 0; j < nodes.Count; j++)
        {
            int posX = (int)nodes[j].x;
            int posY = (int)nodes[j].y;
            int posZ = (int)nodes[j].z;

            if (nodeCount % sizeSquared == 0)
            { // clever way to figure out each increase in Layer
                layerCount += 1;
            }

            // If each corner dont make connector piece
            bool corner = false;
            if (nodeCount % sizeSquared == 0 ||
                nodeCount % sizeSquared == (MapSettings.numMapPiecesXZ + 1) ||
                nodeCount % sizeSquared == (sizeSquared - 1) - (MapSettings.numMapPiecesXZ + 1) ||
                nodeCount % sizeSquared == sizeSquared - 1)
            {
                corner = true;
            }


            if (!corner)
            {
                int mapPieceType = (layerCount % 2 == 0) ? 0 : 1;
                int mapPiece = 0; //Map pieces
                int rotation = 1;

                // Rotation calculation
                if ((nodeCount % sizeSquared >= 0 && nodeCount % sizeSquared <= (MapSettings.numMapPiecesXZ + 1)) ||
                    nodeCount % sizeSquared >= (sizeSquared - 1) - (MapSettings.numMapPiecesXZ + 1) && nodeCount % sizeSquared <= sizeSquared - 1)
                {
                    rotation = 0;
                }

                layers = GetConnectorPiece(mapPieceType, mapPiece);
                int rotations = rotation;

                int objectsCountX = posX;
                int objectsCountY = posY;
                int objectsCountZ = posZ;

                for (int y = 0; y < layers.Count; y++)
                {

                    objectsCountX = posX;
                    objectsCountZ = posZ;

                    floor = layers[y];

                    for (int r = 0; r < rotations; r++)
                    {
                        floor = TransposeArray(floor, MapSettings.sizeOfMapPiecesXZ - 1);
                    }

                    for (int z = 0; z < floor.GetLength(0); z++)
                    {

                        objectsCountX = posX;

                        for (int x = 0; x < floor.GetLength(1); x++)
                        {

                            int cubeType = floor[z, x];
                            GridLoc = new Vector3(objectsCountX, objectsCountY, objectsCountZ);

                            CubeLocationScript cubeScript = LocationManager.GetLocationScript(GridLoc);

                            if (cubeScript != null)
                            {
                                _cubeBuilder.CreateCubeObject(GridLoc, cubeType, rotations, layerCount); // Create the cube
                            }

                            objectsCountX += 1;
                        }
                        objectsCountZ += 1;
                    }
                    objectsCountY += 1;
                }
            }
            nodeCount += 1;
            */

            yield return new WaitForSeconds(waitTime);
            /*
        }

        //GameManager.MapsFinishedLoading ();
        */
    }

    // Get map by type and piece
    private List<int[,]> GetConnectorPiece(int type, int map)
    {
        switch (type)
        {
            case 0: // Floor
                BaseShipPiece connectPiece = null;
                switch (map)
                {
                    case 0:
                       //connectPiece = ScriptableObject.CreateInstance<ConnectorData_01>();
                        break;
                    case 1:
                        //connectPiece = ScriptableObject.CreateInstance<ConnectorData_01>();
                        break;
                    default:
                        break;
                }
                return connectPiece.floors;

            case 1:
                BaseShipPiece ventPiece = null;
                switch (map)
                {
                    case 0:
                        //ventPiece = ScriptableObject.CreateInstance<ConnectorVents_01>();
                        break;
                    case 1:
                       // ventPiece = ScriptableObject.CreateInstance<ConnectorVents_01>();
                        break;
                    default:
                        break;
                }
                return ventPiece.floors;

            default:
                Debug.LogError("OPSALA SOMETHING WRONG HERE!");
                return null;
        }
    }





    private int[,] TransposeArray(int[,] array, int size)
    {

        int[,] ret = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                ret[i, j] = array[size - j - 1, i];
            }
        }

        return ret;
    }
}
