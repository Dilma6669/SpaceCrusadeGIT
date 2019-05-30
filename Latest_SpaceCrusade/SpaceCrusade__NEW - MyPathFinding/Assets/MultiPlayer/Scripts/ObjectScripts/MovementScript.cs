using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    ////////////////////////////////////////////////

    private bool moveInProgress = false;

    private Vector3 _unitCurrPos;

    private List<Vector3> _nodes;


    private Vector3 _finalTargetStaticLoc;              // final target
    private Vector3 _currTargetStaticLoc;               // current target
    private CubeLocation_SERVER _currTargetScript;      // current target script
    public Vector3 _currTargetDynamicLoc;               // vector of the moving cube Object

    public GameObject _worldNodeObject;
    public Vector3 _worldNodeDynamicLoc;                // When the node is moving
    public Vector3 _worldNodeStaticLoc;                 // original position as in when world first loads

    private int locCount;

    private int _unitsSpeed;

    private bool _newPathSet = false;
    private List<Vector3> _newPathNodes;

    private Animator[] _animators;

    private Transform unitContainerTransform; // for rotations

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        unitContainerTransform = transform.Find("UnitContainer");
        _animators = GetComponentsInChildren<Animator>();
    }

    void Start()
    {
        _nodes = new List<Vector3>();
        _newPathNodes = new List<Vector3>();
    }


    // Use this for initialization
    void Update()
    {

        if (moveInProgress)
        {
            StartMoving();
        }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    private Vector3 GetWorldNodeMovementVect()
    {
        return _worldNodeObject.transform.position - _worldNodeStaticLoc;
    }

    private void StartMoving()
    {
        if (locCount < _nodes.Count)
        {
            _unitCurrPos = unitContainerTransform.position;

            _currTargetDynamicLoc = _currTargetStaticLoc + GetWorldNodeMovementVect();

            if (_unitCurrPos != _currTargetDynamicLoc)
            {
                UnitMoveTowardsTarget();
            }
            else
            {
                UnitReachedTarget();
            }
        }
    }

    ////////////////////////////////////////////////

    private void UnitMoveTowardsTarget()
    {
        // Rotation
        Vector3 targetDir = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        //if (_staticTargetVect == Vector3.zero)
        //{
            targetDir = _currTargetDynamicLoc - _unitCurrPos; // for units (rotates almost straight away)
            newDir = Vector3.RotateTowards(unitContainerTransform.forward, targetDir, (Time.deltaTime * 2f) * _unitsSpeed, 0.0f);
        //}
        unitContainerTransform.rotation = Quaternion.LookRotation(newDir);

        // Moving
        transform.position = Vector3.MoveTowards(_unitCurrPos, _currTargetDynamicLoc, Time.deltaTime * _unitsSpeed);
    }

    ////////////////////////////////////////////////

    private void UnitReachedTarget()
    {
        locCount += 1;

        Debug.Log("UnitReachedTarget!");

        if (_newPathSet) // has a new location been clicked
        {
            SetNewpath();
        }
        else if (locCount < _nodes.Count) // move to next target
        {
            SetNextTarget();
        }
        else
        {
            FinishMoving();
        }
    }

    ////////////////////////////////////////////////

    private void SetNewpath()
    {
        Reset();
        _nodes = _newPathNodes;

        _finalTargetStaticLoc = _nodes[_nodes.Count - 1];

        SetNextTarget();
    }

    ////////////////////////////////////////////////

    private void FinishMoving()
    {
        Debug.Log("FINISHED MOVING!");

        AnimationManager.SetAnimatorBool(_animators, "Combat_Walk", false);
        moveInProgress = false;
        Reset();
    }

    ////////////////////////////////////////////////

    private void SetNextTarget()
    {
        _currTargetStaticLoc = _nodes[locCount];
        _currTargetScript = LocationManager.GetLocationScript_SERVER(_nodes[locCount]);
       
        if (!LocationManager.SetUnitOnCube_SERVER(GetComponent<UnitScript>(), _currTargetStaticLoc))
        {
            Debug.LogWarning("units movement interrupted >> recalculating");
            moveInProgress = false;
            Reset();
            //NetWorkManager.NetworkAgent.ServerTellClientToFindNewPathForUnit(PlayerManager.PlayerAgent.NetID, _finalTargetStaticLoc);
        }
    }

    ////////////////////////////////////////////////

    public void MoveUnit(List<Vector3> path)
    {
        AnimationManager.SetAnimatorBool(_animators, "Combat_Walk", true);

        _unitsSpeed = gameObject.transform.GetComponent<UnitScript>().UnitCombatStats[1]; // movement

        if (path.Count > 0)
        {
            _finalTargetStaticLoc = path[path.Count - 1];

            Vector3 nodeVect = gameObject.GetComponent<UnitScript>().NodeID_UnitIsOn;
            //_worldNodeObject = LocationManager.GetNetworkNodeContainerScript_SERVER(nodeVect).gameObject; << network node shit
            _worldNodeStaticLoc = nodeVect;

            if (moveInProgress)
            {
                _newPathNodes = path;
                _newPathSet = true;
            }
            else
            {
                Reset();
                _nodes = path;
                moveInProgress = true;
                SetNextTarget();
            }
        }
    }

    ////////////////////////////////////////////////

    private void Reset()
    {
        locCount = 0;
        //_dynamicTargetLocVect = Vector3.zero;
        //_staticTargetVect = new KeyValuePair<Vector3, Vector3>();
        //_staticFinalTargetVect = new KeyValuePair<Vector3, Vector3>();
        _newPathSet = false;
        //foreach (Vector3 nodeVect in _nodes)
        //{
        //    CubeLocation_SERVER script = LocationManager.GetLocationScript_SERVER(nodeVect);
        //    script.DestroyPathFindingNode();
        //}
        _nodes.Clear();
    }
}
