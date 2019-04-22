using UnityEngine;
using UnityEngine.Networking;

public class CameraAgent : NetworkBehaviour
{
    ////////////////////////////////////////////////

    [HideInInspector]
    public Transform _cameraPivot;
    [HideInInspector]
    public Transform _cameraObject;
    [HideInInspector]
    public Camera _camera;
    [HideInInspector]
    public CameraPivot _cameraPivotScript;


    ////////////////////////////////////////////////

    private Transform _unitPivot; // pivot on unit

    private Transform _cameraPivotTransform;
    private Transform cameraTransform;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        _cameraPivot = GameObject.Find("CameraPivot").transform;
        _cameraObject = GameObject.Find("CameraObject").transform;
        _camera = _cameraObject.GetComponent<Camera>();
        _cameraPivotScript = _cameraPivot.GetComponent<CameraPivot>();
        _camera.enabled = false;
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CameraManager.Camera_Agent = this;
        _camera.enabled = true;

    }



    public void SetCamToOrbitUnit(Transform unit)
    {
        _unitPivot = unit.GetComponent<UnitScript>().PlayerPivot;
        _cameraPivot.transform.SetParent(_unitPivot);
        _cameraPivot.transform.localPosition = Vector3.zero;

        _cameraPivotScript._unitOrbitCamEnable = true;
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
}