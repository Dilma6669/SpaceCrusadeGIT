using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static GridBuilder _instance;

    ////////////////////////////////////////////////

    private static BaseNode _node;

    private static List<Vector3> _GridMovableLocLookup;     // moveable
    private static List<Vector3> _GridNotMovableLocLookup;     // not movable

    private static List<Vector3> _GridStartNodePositions; // important for mapbuilding

    private static int _worldNodeSize = 0;

    ////////////////////////////////////////////////

    public static bool _debugGridObjects; // debugging purposes
    public static bool _debugNodeSpheres = false;

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
        _GridMovableLocLookup = new List<Vector3>();
        _GridNotMovableLocLookup = new List<Vector3>();
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    // NOTE::: ALL THESE LISTS ARE GENERATED EACH MAP PIECE
    // Just movable locations
    public static List<Vector3> GetGrid_MOVEABLE_LocLookup()
    {
        return _GridMovableLocLookup;
    }
    // Just NOT movable locations
    public static List<Vector3> GetGrid_NOTMOVEABLE_Lookup(Vector3 vect)
    {
        return _GridNotMovableLocLookup;
    }
    // Bottom corner start positions
    public static List<Vector3> GetGridNodeStartPositions()
    {
        return _GridStartNodePositions;
    }

    ////////////////////////////////////////////////s

    public static void BuildLocationGrid<T>(T node, int worldNodeSize) where T : BaseNode
    {
        // these need to be reset each map piece built
        _GridStartNodePositions = new List<Vector3>();
        _GridMovableLocLookup = new List<Vector3>();
        _GridNotMovableLocLookup = new List<Vector3>();

        _node = node as T;

        _worldNodeSize = worldNodeSize;

        // these are the bottom left corner axis for EACH map node
        int startGridLocX = (int)node.NodeStaticLocation.x - (MapSettings.sizeOfMapPiecesXZ / 2);
        int startGridLocY = (int)node.NodeStaticLocation.y - (MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY) / 2;
        int startGridLocZ = (int)node.NodeStaticLocation.z - (MapSettings.sizeOfMapPiecesXZ / 2);

        BuildGridLocations(startGridLocX, startGridLocY, startGridLocZ);

    }



	public static void BuildGridLocations(int startX, int startY, int startZ) {

        int gridLocX = startX;
		int gridLocY = startY;
		int gridLocZ = startZ;

        int finishX = startX + 24;
        int finishY = startY + 6;
        int finishZ = startZ + 24;

		// Floors layer
		for (int y = startY; y < finishY; y++) { // this needs attention!!!!

			gridLocX = startX;
			gridLocZ = startZ;

            for (int z = startZ; z < finishZ; z++) {

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

                    // Create location positions
                    ///////////////////////////////
                    // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                    Vector3 gridLoc = new Vector3(gridLocX, gridLocY, gridLocZ);

                    PutGridLocInCorrectLists(gridLoc, startY);

                    gridLocX += 1;
				}
				gridLocZ += 1;
			}
			gridLocY += 1;
		}

   
		// Vents layer
		// An attempt to build the vents layer //seems to be working
		gridLocX = startX;
		//gridLocY = gridLocY;
		gridLocZ = startZ;

        startY += MapSettings.sizeOfMapPiecesY;

        int offset = 0;

        // for an extra layer roof of the vents only appearing if no map piece above vent
        if (_node.neighbours[5] == -1)
        {
            offset = 1;
        }

        for (int y = startY; y < (startY + MapSettings.sizeOfMapVentsY) + offset; y++) {

            gridLocX = startX;
			gridLocZ = startZ;

			for (int z = startZ; z < finishZ; z++) {

				gridLocX = startX;

				for (int x = startX; x < finishX; x++) {

                    // Create location positions
                    ///////////////////////////////
                    // put vector location, eg, grid Location 0,0,0 and World Location 35, 0, 40 value pairs into hashmap for easy lookup
                    Vector3 gridLoc = new Vector3(gridLocX, gridLocY, gridLocZ);

                    PutGridLocInCorrectLists(gridLoc, startY);

					gridLocX += 1;
				}
				gridLocZ += 1;
			}
			gridLocY += 1;
		}
    }

    // node objects are spawned at bottom corner each map piece
    private static void PutGridLocInCorrectLists(Vector3 gridLoc, int startY)
    {
        if (gridLoc.x % 2 == 0 && gridLoc.y % 2 == 1)
        {
            // movable locations
            _GridMovableLocLookup.Add(gridLoc);
        }
        else
        {
            // non moveable loacatoins
            _GridNotMovableLocLookup.Add(gridLoc);
        }

        // for the corner map nodes for mapbuilding
        int multiple = (_worldNodeSize * MapSettings.sizeOfMapPiecesXZ) / _worldNodeSize;

        if (gridLoc.x % multiple == 0 && gridLoc.z % multiple == 0 && gridLoc.y == startY)
        {
            _GridStartNodePositions.Add(gridLoc);
        }

        // Create empty objects at locations to see the locations (debugging purposes)
        if (_debugGridObjects)
        {
            WorldBuilder._nodeBuilder.InstantiateNodeObject(gridLoc, NodeTypes.GridNode, WorldManager._GridContainer.transform);
        }
    }
}
	

