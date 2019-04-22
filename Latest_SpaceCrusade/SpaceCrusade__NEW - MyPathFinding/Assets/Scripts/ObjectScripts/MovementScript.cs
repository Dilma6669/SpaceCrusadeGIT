using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    ////////////////////////////////////////////////

    private bool moveInProgress = false;

    private Vector3 _unitCurrPos;

    private List<KeyValuePair<Vector3, Vector3>> _nodes;

    private KeyValuePair<Vector3, Vector3> _staticFinalTargetVect;

    private KeyValuePair<Vector3, Vector3> _staticTargetVect;
    private CubeLocationScript _staticTargetScript;
    private Vector3 _dynamicTargetLocVect;

    private bool collision = false;

    private int locCount;

    private int _unitsSpeed;

    private bool _newPathSet = false;
    private List<KeyValuePair<Vector3, Vector3>> _newPathNodes;

    private Animator[] _animators;

    private Transform unitContainerTransform; // for rotations

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        unitContainerTransform = GameObject.Find("UnitContainer").transform;
        _animators = GetComponentsInChildren<Animator>();
    }

    void Start()
    {
        _nodes = new List<KeyValuePair<Vector3, Vector3>>();
        _newPathNodes = new List<KeyValuePair<Vector3, Vector3>>();
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

    private void StartMoving()
    {
        if (locCount < _nodes.Count)
        {
            _unitCurrPos = unitContainerTransform.position;

            _dynamicTargetLocVect = _staticTargetScript.transform.position;

            if (_unitCurrPos != _dynamicTargetLocVect)
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
        AnimationManager.SetAnimatorBool(_animators, "Combat_Walk", true);

        // Rotation
        Vector3 targetDir = Vector3.zero;
        Vector3 newDir = Vector3.zero;
        if (_staticTargetVect.Value == Vector3.zero)
        {
            targetDir = _dynamicTargetLocVect - _unitCurrPos; // for units (rotates almost straight away)
            newDir = Vector3.RotateTowards(unitContainerTransform.forward, targetDir, (Time.deltaTime * 2f) * _unitsSpeed, 0.0f);
        }
        unitContainerTransform.rotation = Quaternion.LookRotation(newDir);

        // Moving
        transform.position = Vector3.MoveTowards(_unitCurrPos, _dynamicTargetLocVect, Time.deltaTime * _unitsSpeed);
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

        _staticFinalTargetVect = _nodes[_nodes.Count - 1];

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
        _staticTargetVect = _nodes[locCount];
        _staticTargetScript = LocationManager.GetLocationScript(_nodes[locCount].Key);

        if (!LocationManager.SetUnitOnCube(GetComponent<UnitScript>(), _staticTargetVect.Key))
        {
            Debug.LogWarning("units movement interrupted >> recalculating");
            moveInProgress = false;
            Reset();
            UnitsManager.MakeUnitRecalculateMove(GetComponent<UnitScript>(), _staticFinalTargetVect);
        }
    }

    ////////////////////////////////////////////////

    public void MoveUnit(List<KeyValuePair<Vector3, Vector3>> _posRot)
    {
        Debug.Log("MoveUnit!");

        _unitsSpeed = gameObject.transform.GetComponent<UnitScript>().UnitCombatStats[1]; // movement

        if (_posRot.Count > 0)
        {
            _staticFinalTargetVect = _posRot[_posRot.Count - 1];
            _staticTargetScript = LocationManager.GetLocationScript(_posRot[0].Key);

            if (moveInProgress)
            {
                _newPathNodes = _posRot;
                _newPathSet = true;
            }
            else
            {
                Reset();
                _nodes = _posRot;
                moveInProgress = true;
                SetNextTarget();
            }
        }
    }

    ////////////////////////////////////////////////

    private void Reset()
    {
        locCount = 0;
        _dynamicTargetLocVect = Vector3.zero;
        _staticTargetVect = new KeyValuePair<Vector3, Vector3>();
        _staticFinalTargetVect = new KeyValuePair<Vector3, Vector3>();
        _newPathSet = false;
        foreach (KeyValuePair<Vector3, Vector3> nodeVect in _nodes)
        {
            CubeLocationScript script = LocationManager.GetLocationScript(nodeVect.Key);
            script.DestroyPathFindingNode();
        }
        _nodes.Clear();
    }


}
