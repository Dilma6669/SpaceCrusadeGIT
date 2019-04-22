using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static CameraManager _instance;

    private static CameraAgent _cameraAgent;

    ////////////////////////////////////////////////

    public static CameraAgent Camera_Agent
    {
        get { return _cameraAgent; }
        set { _cameraAgent = value; }
    }

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

    public static void SetUpCameraAndLayers(int playerID, CameraAgent cameraAgent)
    {
        Camera_Agent = cameraAgent;


        KeyValuePair<Vector3, Vector3> camStartPos = GetCameraStartPosition(playerID);

        Vector3 camPos = camStartPos.Key;
        Quaternion camRot = Quaternion.Euler(camStartPos.Value);

        PlayerManager.PlayerAgent.SetUpPlayerStartPosition(camPos, camRot);

        Camera_Agent._cameraPivotScript.angleH = camRot.eulerAngles.y;
        Camera_Agent._cameraPivotScript.angleV = -camRot.eulerAngles.x;

        /*

        // reveal layers up to current
        for (int i = 0; i <= _currLayer; i++) 
        {
            _camera.cullingMask |= 1 << LayerMask.NameToLayer("Floor" + i.ToString ());
        }
        */

        // units have already been put into correct layer now need to make camera see layer
        //string layerStr = "Player0" + playerID.ToString () + "Units";
        //_camera.cullingMask |= 1 << LayerMask.NameToLayer (layerStr);
    }

    public static KeyValuePair<Vector3, Vector3> GetCameraStartPosition(int playerID = -1)
    {
        return PlayerManager.GetPlayerStartPosition(playerID);
    }

    public static void SetCamToOrbitUnit(Transform unit)
    {
        Camera_Agent.SetCamToOrbitUnit(unit);
    }

}
