using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LayerManager _instance;

    ////////////////////////////////////////////////

    // Layer INfo
    private static int _startLayer;
    private static int _maxLayer; // This needs to change with the amout of y levels, basicly level*2 because of vents layer ontop of layer
    private static int _minLayer;
    private static int _currLayer;

    ////////////////////////////////////////////////

    public static int LayerStart
    {
        get { return _startLayer; }
        set { _startLayer = value; }
    }
    public static int LayerMax
    {
        get { return _maxLayer; }
        set { _maxLayer = value; }
    }
    public static int LayerMin
    {
        get { return _minLayer; }
        set { _minLayer = value; }
    }
    public static int LayerCurr
    {
        get { return _currLayer; }
        set { _currLayer = value; }
    }

    ////////////////////////////////////////////////

    public static Dictionary<int, List<CubeLocationScript>> _layerCubeList;
    public static Dictionary<int, List<BaseNode>> _layerNodeList;

    static int layerID;
    static BaseNode parentNode;
    static int parentLayerID;


    static int LAYERVISIBLE = 8;
    static int LAYERNOTVISIBLE = 9;

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
        _layerCubeList = new Dictionary<int, List<CubeLocationScript>>();
        _layerNodeList = new Dictionary<int, List<BaseNode>>();

        // change layer callback event
        UIManager.OnChangeLayerClick += ChangeCameraLayerByHUD;

        LayerCurr = -1;
    }

    //////////////////

    public static void ChangeCameraLayerByHUD(int change)
    {
        if (change == 1)
        {
            layerID++;
            ChangeSpecificCubesVisibility(LAYERVISIBLE);
        }
        if (change == -1)
        {
            ChangeSpecificCubesVisibility(LAYERNOTVISIBLE);
            layerID--;
        }
        LayerCurr = layerID;
    }


    public static void ChangeCameraLayer(CubeLocationScript cubeScript)
    {
        BaseNode parentWorldNode = cubeScript.transform.parent.parent.GetComponent<BaseNode>();
        BaseNode parentMapNode = cubeScript.transform.parent.GetComponent<BaseNode>();

        if (parentWorldNode.entrance)
        {
            parentNode = parentWorldNode;
        }
        else
        {
            parentNode = parentMapNode;
        }
        parentLayerID = parentNode.NodeLayerCount;

        ChangeSpecificNodeVisibility(LAYERNOTVISIBLE); // set specfic world node to not visible, basicly clears all layers so correct layer can be set

        if (LayerCurr != -1)
        {
            //ChangeSpecificNodeVisibility(parentWorldNode, LAYERVISIBLE);
            //ChangeSpecificCubesVisibility(parentWorldNode, LAYERNOTVISIBLE);
        }
    
        layerID = cubeScript.CubeLayerID;

        for (int i = parentLayerID; i <= cubeScript.CubeLayerID; i++)
        {
            layerID = i;
            ChangeSpecificCubesVisibility(LAYERVISIBLE);
        }

        LayerCurr = layerID;
    }



    public static void ChangeSpecificNodeVisibility(int visibilityLayer)
    {
        parentNode.gameObject.layer = visibilityLayer;

        List<GameObject> units = new List<GameObject>(); 

        Transform[] allChildren = parentNode.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            child.gameObject.layer = visibilityLayer;

            if (child.GetComponent<UnitScript>()) // for the units
            {
                units.Add(child.gameObject);
            }
        }

        foreach(GameObject unit in units)
        {
            AssignUnitToLayer(unit);
        }

    }

    public static void ChangeSpecificCubesVisibility(int visibilityLayer)
    {
        CubeLocationScript[] allChildren = parentNode.GetComponentsInChildren<CubeLocationScript>();

        foreach (CubeLocationScript child in allChildren)
        {
            if (child.CubeLayerID == layerID)
            {
                child.gameObject.layer = visibilityLayer;

                Transform[] grandChildren = child.GetComponentsInChildren<Transform>();
                foreach (Transform grand in grandChildren)
                {
                    grand.gameObject.layer = visibilityLayer;
                }
            }
        }
    }


    //////////////////

    public static void AddCubeToLayer(CubeLocationScript script)
    {
        int cubeLayer = script.CubeLayerID;
        if (_layerCubeList.ContainsKey(cubeLayer))
        {
            _layerCubeList[cubeLayer].Add(script);
        }
        else
        {
            _layerCubeList.Add(cubeLayer, new List<CubeLocationScript> { script });
        }
    }

    public static void ChangeCubeLayerVisibility(int layerID, int visibilityLayer)
    {
        if (_layerCubeList.ContainsKey(layerID))
        {
            List<CubeLocationScript> scripts = _layerCubeList[layerID];

            foreach (CubeLocationScript script in scripts)
            {
                script.gameObject.layer = visibilityLayer;

                Transform[] allChildren = script.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    child.gameObject.layer = visibilityLayer;
                }
            }
        }
        else
        {
            Debug.LogError("Got an issue here NO layerID assigned or not in list: " + layerID);
        }
    }


    public static void MakeAllCubeLayersVisible()
    {
        Debug.Log("MakeAllLayersVisible");
        foreach (int layerID in _layerCubeList.Keys)
        {
            ChangeCubeLayerVisibility(layerID, LAYERVISIBLE);
        }
    }

    public static void MakeAllCubeLayersNotVisible()
    {
        Debug.Log("MakeAllLayersNotVisible");
        foreach (int layerID in _layerCubeList.Keys)
        {
            ChangeCubeLayerVisibility(layerID, LAYERNOTVISIBLE);
        }
    }

    //////////////////

    public static void AddNodeToLayer(BaseNode node)
    {
        int nodeLayer = node.NodeLayerCount;
        if (_layerNodeList.ContainsKey(nodeLayer))
        {
            _layerNodeList[nodeLayer].Add(node);
        }
        else
        {
            _layerNodeList.Add(nodeLayer, new List<BaseNode> { node });
        }
    }

    public static void ChangeNodeLayerVisibility(int layerID, int visibilityLayer)
    {
        if (_layerNodeList.ContainsKey(layerID))
        {
            List<BaseNode> nodes = _layerNodeList[layerID];

            foreach (BaseNode node in nodes)
            {
                node.gameObject.layer = visibilityLayer;

                Transform[] allChildren = node.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    child.gameObject.layer = visibilityLayer;
                }
            }
        }
        else
        {
            Debug.LogError("Got an issue here NO layerID assigned or not in list: " + layerID);
        }
    }

    public static void MakeAllNodeLayersVisible()
    {
        Debug.Log("MakeAllLayersVisible");
        foreach (int layerID in _layerNodeList.Keys)
        {
            ChangeNodeLayerVisibility(layerID, LAYERVISIBLE);
        }
    }

    public static void MakeAllNodeLayersNotVisible()
    {
        Debug.Log("MakeAllLayersNotVisible");
        foreach (int layerID in _layerNodeList.Keys)
        {
            ChangeNodeLayerVisibility(layerID, LAYERNOTVISIBLE);
        }
    }


    //////////////////

    public static void AssignUnitToLayer(GameObject unit)
    {
        int playerID = PlayerManager.PlayerID;
        int unitControllerID = unit.GetComponent<UnitScript>().PlayerControllerID;

        if (playerID == unitControllerID)
        {
            unit.layer = LAYERVISIBLE;

            Transform[] allChildren = unit.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                child.gameObject.layer = LAYERVISIBLE;
            }
        }
        else
        {
            unit.layer = LAYERNOTVISIBLE;

            Transform[] allChildren = unit.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                child.gameObject.layer = LAYERNOTVISIBLE;
            }
        }
    }







}
