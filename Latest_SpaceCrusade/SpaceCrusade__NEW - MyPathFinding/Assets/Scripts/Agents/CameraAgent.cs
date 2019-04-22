using UnityEngine;
using UnityEngine.Networking;

public class CameraAgent : NetworkBehaviour
{
    ////////////////////////////////////////////////

    public Transform _cameraPivot;
    public Transform _cameraObject;
    public Camera _camera;

    [HideInInspector]
    public CameraPivot _cameraPivotScript;

    ////////////////////////////////////////////////

    private Transform _cameraPivotTransform;
    private Transform cameraTransform;

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    void Awake()
    {
        _cameraPivot = transform.Find("CameraPivot");
        if (_cameraPivot == null) { Debug.LogError("We got a problem here");}

        _cameraObject = transform.FindDeepChild("CameraObject");
        if (_cameraObject == null) { Debug.LogError("We got a problem here");}

        _camera = _cameraObject.GetComponent<Camera>();
        if (_camera == null) { Debug.LogError("We got a problem here"); }

        _cameraPivotScript = _cameraPivot.GetComponent<CameraPivot>();
        _camera.enabled = false;
    }

    void Start()
    {
        if (!isLocalPlayer) return;

        CameraManager.Camera_Agent = this;
        _camera.enabled = true;

    }



    public void SetCamAgentToOrbitUnit(UnitScript unitScript)
    {
        if (!isLocalPlayer) return;

        if (unitScript.PlayerPivot == null)
        {
            print("ERROR UnitScript.PlayerPivot == null unitScript.NetID.Value: " + unitScript.NetID.Value);
        }
        _cameraPivotScript.SetNewPivot(unitScript.PlayerPivot);
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////
}