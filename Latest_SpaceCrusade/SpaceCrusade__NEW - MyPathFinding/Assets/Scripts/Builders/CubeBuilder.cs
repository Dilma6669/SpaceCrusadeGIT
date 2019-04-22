using UnityEngine;

public class CubeBuilder : MonoBehaviour {

    ////////////////////////////////////////////////

    private static CubeBuilder _instance;

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

    public static CubeLocationScript CreateCubeObject(Vector3 gridLoc, int cubeType, int rotations, int nodeLayerCount, Transform parent)
    {
        GameObject cubeObject = WorldBuilder._nodeBuilder.CreateDefaultCube(gridLoc, rotations, nodeLayerCount, parent);
        CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript>();

        switch (cubeType)
        {
            case 00:
                break;
            case 01:
                PanelBuilder.CreatePanelForCube("Floor", cubeScript, 0, rotations);
                break;
            case 02:
                PanelBuilder.CreatePanelForCube("Wall", cubeScript, 90, rotations); // Down
                break;
            case 03:
                PanelBuilder.CreatePanelForCube("Wall", cubeScript, 0, rotations); // across
                break;
            case 04:
                PanelBuilder.CreatePanelForCube("FloorAngle", cubeScript, 90, rotations);
                break;
            case 05:
                PanelBuilder.CreatePanelForCube("FloorAngle", cubeScript, 270, rotations);
                break;
            case 06:
                PanelBuilder.CreatePanelForCube("FloorAngle", cubeScript, 180, rotations);
                break;
            case 07:
                PanelBuilder.CreatePanelForCube("FloorAngle", cubeScript, 0, rotations);
                break;
            case 08:
                PanelBuilder.CreatePanelForCube("CeilingAngle", cubeScript, 90, rotations);
                break;
            case 09:
                PanelBuilder.CreatePanelForCube("CeilingAngle", cubeScript, 270, rotations);
                break;
            case 10:
                PanelBuilder.CreatePanelForCube("CeilingAngle", cubeScript, 180, rotations);
                break;
            case 11:
                PanelBuilder.CreatePanelForCube("CeilingAngle", cubeScript, 0, rotations);
                break;
        }

        SortOutCubeScriptShit(gridLoc, cubeScript);

        return cubeScript;
    }


    private static void SortOutCubeScriptShit(Vector3 GridLoc, CubeLocationScript cubeScript)
    {
        // If cube is movable or not
        if (cubeScript.CubeMoveable)
        {
          LocationManager.SetCubeScriptToLocation(GridLoc, cubeScript);
        }
        else
        {
            LocationManager.SetCubeScriptToHalfLocation(GridLoc, cubeScript);
        }

        // for layering system
        LayerManager.AddCubeToLayer(cubeScript);
    }

    ////////////////////////////////////////////////

}
