using UnityEngine;

public class NodeBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static NodeBuilder _instance;

    ////////////////////////////////////////////////

    public GameObject _defaultCubeObject; // THE CUBES EVERYWHERE
    public GameObject _gridObjectPrefab; // Debugging purposes
    public GameObject _worldNodePrefab; // object that shows Map nodes
    public GameObject _mapNodePrefab; // object that shows Map nodes
    public GameObject _connectorNodePrefab; // object that shows Map nodes

    public GameObject _normalCoverPrefab;
    public GameObject _openCoverPrefab;
    public GameObject _largeGarageCoverPrefab;
    public GameObject _connectorCoverPrefab;

    public GameObject _motherShip;

    public GameObject _panelPrefab;

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

        if (_defaultCubeObject == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_gridObjectPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_worldNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_mapNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_connectorNodePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (_normalCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_openCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_largeGarageCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
        if (_connectorCoverPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (_motherShip == null) { Debug.LogError("OOPSALA we have an ERROR!"); }

        if (_panelPrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public GameObject CreateDefaultCube(Vector3 gridLoc, int rotations, int nodeLayerCount, Transform parent)
    {
        GameObject cubeObject = Instantiate(GetNodePrefab(NodeTypes.CubeObject), parent, false); // empty cube
        cubeObject.transform.SetParent(parent);
        cubeObject.transform.position = gridLoc;
        int rotationY = (rotations * -90) % 360;
        cubeObject.transform.eulerAngles = new Vector3(0, rotationY, 0);

        CubeLocationScript cubeScript = cubeObject.GetComponent<CubeLocationScript>();
        cubeScript.CubeMoveable = (gridLoc.x % 2 == 0 && gridLoc.y % 2 == 1) ? true : false;
        cubeScript.CubeStaticLocVector = gridLoc;
        cubeScript.CubeLayerID = nodeLayerCount;
        cubeScript.CubeAngle = rotationY;

        return cubeObject;
    }

    ////////////////////////////////////////////////

    // Create Generic Node /////////////////////////////////////////////////////
    public T CreateNode<T>(Transform parentNode, Vector3 vect, int rotation, int mapType, int mapPiece, NodeTypes nodeType) where T : BaseNode
    {
        //Debug.Log("Vector3 (gridLoc): x: " + vect.x + " y: " + vect.y + " z: " + vect.z);
        GameObject node = InstantiateNodeObject(vect, nodeType, parentNode);
        T nodeScript = node.GetComponent<T>();
        nodeScript.NodeStaticLocation = vect;
        nodeScript.NodeStaticRotation = rotation;
        nodeScript.NodeMapType = mapType;
        nodeScript.NodeMapPiece = mapPiece;
        nodeScript.NodeLayerCount = -1;
        nodeScript.NodeID = vect;
        return nodeScript;
    }

    // node objects are spawned at bottom corner each map piece
    public GameObject InstantiateNodeObject(Vector3 loc, NodeTypes nodePrefab, Transform parent)
    {
        //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
        GameObject nodeObject = Instantiate(GetNodePrefab(nodePrefab), parent, false);
        nodeObject.transform.position = loc;
        nodeObject.transform.SetParent(parent);
        nodeObject.transform.localScale = new Vector3(1, 1, 1);

        return nodeObject;
    }

    private GameObject GetNodePrefab(NodeTypes node)
    {
        switch (node)
        {
            case NodeTypes.CubeObject:
                return _defaultCubeObject;
            case NodeTypes.GridNode:
                return _gridObjectPrefab;
            case NodeTypes.WorldNode:
                return _worldNodePrefab;
            case NodeTypes.MapNode:
                return _mapNodePrefab;
            case NodeTypes.ConnectorNode:
                return _connectorNodePrefab;
            default:
                Debug.Log("OPPSALA WE HAVE AN ISSUE HERE");
                return null;
        }
    }

    ////////////////////////////////////////////////

    public void AttachCoverToNode<T>(T nodeType, GameObject node, CoverTypes _cover, Vector3 rotation) where T : BaseNode
    {
        //Debug.Log("Vector3 (gridLoc): x: " + gridLocX + " y: " + gridLocY + " z: " + gridLocZ);
        GameObject cover = Instantiate(GetCoverPrefab(_cover), node.transform, false);
        cover.transform.SetParent(node.transform);
        cover.GetComponent<NodeCover>().parentNode = nodeType;
        cover.transform.localEulerAngles = rotation;
    }


    private GameObject GetCoverPrefab(CoverTypes cover)
    {
        switch (cover)
        {
            case CoverTypes.NormalCover:
                return _normalCoverPrefab;
            case CoverTypes.OpenCover:
                return _openCoverPrefab;
            case CoverTypes.LargeGarageCover:
                return _largeGarageCoverPrefab;
            case CoverTypes.ConnectorCover:
                return _connectorCoverPrefab;
            default:
                Debug.Log("OPPSALA WE HAVE AN ISSUE HERE");
                return null;
        }
    }

    ////////////////////////////////////////////////

    public void CreatePlayersShip(Vector3 loc, Vector3 rot, Transform parent)
    {
        GameObject ship = Instantiate(_motherShip, parent, false);
        ship.transform.position = loc;
        ship.transform.localEulerAngles = rot;
        ship.transform.SetParent(parent);
    }


    public GameObject CreatePanelObject(string panelName, Transform parent)
    {
        GameObject panelObject = Instantiate(_panelPrefab, parent, false);
        panelObject.transform.SetParent(parent);

        PanelPieceScript panelScript = panelObject.gameObject.GetComponent<PanelPieceScript>();
        CubeLocationScript cubeScript = parent.gameObject.GetComponent<CubeLocationScript>();
        cubeScript._panelScriptChild = panelScript;
        cubeScript._isPanel = true;
        panelObject.name = (panelName);

        panelScript.cubeScriptParent = cubeScript;
        panelScript._camera = CameraManager.Camera_Agent._camera;

        return panelObject;
    }

    ////////////////////////////////////////////////
}
