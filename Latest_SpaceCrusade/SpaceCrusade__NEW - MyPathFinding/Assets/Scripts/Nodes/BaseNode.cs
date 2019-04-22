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
    private Rigidbody rigidbody;


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


    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    public virtual bool ActivateMapPiece<T>(T nodeType, bool coverActive, GameObject cover) where T : BaseNode // turn on all map objects
    {
        _nodeCover = cover.gameObject;

        WorldBuilder.AttachMapToNode(nodeType);
        Vector3 testLocVect = new Vector3(500, 0, 0);
        Vector3 testRotVect = new Vector3(0, 0, 0);
        NetWorkManager.NetworkAgent.CmdTellServerToMoveWorldNode(NodeID, testLocVect, testRotVect);

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

    public virtual void EnableNodeRigidBody(bool OnOff)
    {
        if(rigidbody == null)
        {
            gameObject.AddComponent<Rigidbody>();
            rigidbody = GetComponent<Rigidbody>();
        }

        rigidbody.useGravity = false;

        if (OnOff)
        {
            rigidbody.isKinematic = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }
    }

    public virtual void MoveNode(KeyValuePair<Vector3, Vector3> posRot)
    {
        EnableNodeRigidBody(true);

        Vector3 locToMoveto = posRot.Key;
        Vector3 rotToMoveto = posRot.Value;
        rigidbody.AddForce(locToMoveto);
        rigidbody.AddTorque(rotToMoveto);
    }

}
