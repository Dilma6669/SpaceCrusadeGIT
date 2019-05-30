using System.Collections.Generic;
using UnityEngine;

public class BaseNode : MonoBehaviour {

    private Vector3 _nodeID;
    private NodeTypes _thisNodeType;
    private Vector3 _nodeStaticLocation;
    private Vector3 _nodeDynamicLocation;
    private int _nodeSize;
    private int _nodeStaticRotation;
    private Vector3 _nodeDynamicRotation;
    private int _nodeLayerCount;
    private int _nodeMapType;
    private int _nodeMapPiece;

    public int[] neighbours;
    public bool entrance = false;
    //public List<int> entranceSides = new List<int>();

    public bool playerShipMapPART1 = false;
    public bool playerShipMapPART2 = false;

    public bool connectorUp = false;

    public WorldNode worldNodeParent;
    protected GameObject _nodeCover;

    private bool _followNetworkNode = false;
    private GameObject _networkNodeContainer;

    private float _thrust = 0f;

    ////////////////////////////////////////////////

    public Vector3 NodeID
    {
        get { return _nodeID; }
        set { _nodeID = value; }
    }
    public NodeTypes NodeType
    {
        get { return _thisNodeType; }
        set { _thisNodeType = value; }
    }
    public int NodeSize
    {
        get { return _nodeSize; }
        set { _nodeSize = value; }
    }
    public int NodeLayerCount
    {
        get { return _nodeLayerCount; }
        set { _nodeLayerCount = value; }
    }
    public int NodeMapType
    {
        get { return _nodeMapType; }
        set { _nodeMapType = value; }
    }
    public int NodeMapPiece
    {
        get { return _nodeMapPiece; }
        set { _nodeMapPiece = value; }
    }


    public int NodeStaticRotation
    {
        get { return _nodeStaticRotation; }
        set { _nodeStaticRotation = value; }
    }
    public Vector3 NodeStaticLocation
    {
        get { return _nodeStaticLocation; }
        set { _nodeStaticLocation = value; }
    }


    public GameObject NetworkNodeContainer
    {
        get { return _networkNodeContainer; }
        set { _networkNodeContainer = value; }
    }

    void Awake()
    {
    }

    void Update()
    {
        if (_followNetworkNode)
        {
            // Rotation
            //transform.rotation = Vector3.RotateTowards(transform.position, _networkNode.transform.position, (Time.deltaTime * 50f), 0.0f);
            transform.rotation = Quaternion.Euler(NetworkNodeContainer.transform.rotation.eulerAngles);

            // Moving
            transform.position = Vector3.MoveTowards(transform.position, NetworkNodeContainer.transform.position, (Time.deltaTime * _thrust));
        }
    }

    public virtual bool ActivateMapPiece<T>(T nodeType, bool coverActive, GameObject cover) where T : BaseNode // turn on all map objects
    {
        _nodeCover = cover.gameObject;

        WorldBuilder.AttachMapToNode(nodeType);
        //Vector3 testLocVect = new Vector3(200, 0, 0);
        //Vector3 testRotVect = new Vector3(0, 0, 0);
        //float thrust = 5;
        //NetWorkManager.NetworkAgent.CmdTellServerToMoveWorldNode(NodeID, testLocVect, testRotVect, thrust);

        if (coverActive)
        {
            _nodeCover.SetActive(false); // this turns the cover off
            return false; // this is not a fail, this is deactivation
        }
        else
        {
            _nodeCover.SetActive(true);  // this turns the cover On
            return true;
        }
    }


    public void MakeNodeFollowNetworkNode(float thrust)
    {
        _thrust = thrust;
        _followNetworkNode = true;
    }
}
