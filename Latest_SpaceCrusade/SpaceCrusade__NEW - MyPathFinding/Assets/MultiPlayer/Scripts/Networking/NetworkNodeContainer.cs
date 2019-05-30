using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkNodeContainer : NetworkBehaviour
{
    public Vector3 _nodeID;

    ////////////////////////////////////////////////

    private Rigidbody rigidbody;

    private bool _moveNode = false;

    ////////////////////////////////////////////////

    public Vector3 ContainerNodeID
    {
        get { return _nodeID; }
        set { _nodeID = value; }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }


    public virtual void EnableNodeRigidBody(bool OnOff)
    {
        if (rigidbody == null)
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

    public virtual void MoveNetworkNode(KeyValuePair<Vector3, Vector3> posRot)
    {
        EnableNodeRigidBody(true);

        transform.position = posRot.Key;
        transform.rotation = Quaternion.Euler(posRot.Value);
    }
}
