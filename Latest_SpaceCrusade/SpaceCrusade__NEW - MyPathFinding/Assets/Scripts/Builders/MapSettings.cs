using UnityEngine;

public class MapSettings : MonoBehaviour {

    // Kind of Global references //
    public static bool LOADPREBUILT_STRUCTURE = true;

    public static int MAPTYPE_MAP_FLOOR = 0;
    public static int MAPTYPE_MAP_VENTS = 1;
    public static int MAPTYPE_CONNECT_FLOOR = 2;
    public static int MAPTYPE_CONNECT_VENTS = 3;
    public static int MAPTYPE_CONNECT_UP_FLOOR = 4;
    public static int MAPTYPE_CONNECT_UP_VENTS = 5;
    public static int MAPTYPE_SHIPPORT_FLOOR = 6;
    public static int MAPTYPE_SHIPPORT_VENTS = 7;

    //////////////////////////


    private static int _worldSizeX = 10;
    public static int worldSizeX { get { return _worldSizeX; } }

    private static int _worldSizeZ = 10;
    public static int worldSizeZ { get { return _worldSizeZ; } }

    private static int _worldSizeY = 45;
    public static int worldSizeY { get { return _worldSizeY; } }

    private static int _worldType = 0; // 0 = square, 1 = Line, 2 = tower
    public static int worldType { get { return _worldType; } }

    //////////////////////////////

    private static int _sizeOfMapPiecesXZ = 24; // 24
    public static int sizeOfMapPiecesXZ { get { return _sizeOfMapPiecesXZ; } }

    private static int _sizeOfMapPiecesY = 6; // 6
    public static int sizeOfMapPiecesY { get { return _sizeOfMapPiecesY; } }

    private static int _sizeOfMapVentsY = 2; // 2
    public static int sizeOfMapVentsY { get { return _sizeOfMapVentsY; }}




    private static int _worldNodeDistanceXZ = 2; // 1 less than max map size. Space inbetween nodes. needs a +1 to get new location
    public static int worldNodeDistanceXZ { get { return _worldNodeDistanceXZ; } }

    private static int _worldNodeDistanceY = 2; // Space inbetween nodes. needs a +1 to get new location
    public static int worldNodeDistanceY { get { return _worldNodeDistanceY; } }




    private static int _worldPadding = -20; // 10 * 24 = nodes start at X : 240
    public static int worldPadding { get { return _worldPadding; } }

    private static int _sizeOfCubes = 1; // 1
    public static int sizeOfCube { get { return _sizeOfCubes; } }

    private static int[] sizes;
    public static int getRandomMapSize { get { return sizes[Random.Range(0, sizes.Length)]; } }

    private static int _sizeOfMapConnectorsXYZ = 1; // 1
    public static int sizeOfMapConnectorsXYZ { get { return _sizeOfMapConnectorsXYZ; } }


    void Awake() {
        sizes = new int[] { 1, 3 };
    }


}
