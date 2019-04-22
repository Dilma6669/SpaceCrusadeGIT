using UnityEngine;

public class PanelBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static PanelBuilder _instance;

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

    public static void CreatePanelForCube(string panelName, CubeLocationScript cubeScript, int angle, int rotations) {

        Transform cubeTrans = cubeScript.GetComponent<Transform>();
        GameObject panelObject = WorldBuilder._nodeBuilder.CreatePanelObject(panelName, cubeTrans);
        PanelPieceScript panelScript = panelObject.GetComponent<PanelPieceScript>();

        switch (panelName) {
		case "Floor":
			panelObject.transform.localPosition = new Vector3 (0, 0, 0);
			panelObject.transform.localEulerAngles = new Vector3 (90, angle, 0);
			panelObject.transform.tag = ("Panel_Floor");
			break;
		case "Wall":
			panelObject.transform.localPosition = new Vector3 (0, 0, 0);
			if (rotations == 0) { // Seems good
				if (angle == 0) { // across
					if (cubeScript.CubeAngle == 0) {
						angle = 180;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 180;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 0;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else if (angle == 90) { // Down
					if (cubeScript.CubeAngle == 0) {
						angle = 270;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 270;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 90;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else {
					Debug.Log ("Got a wierd issue here!!");
				}
			} else if (rotations == 1) {
				if (angle == 0) { // across
					if (cubeScript.CubeAngle == 0) {
						angle = 180;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 180;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 0;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else if (angle == 90) { // Down
					if (cubeScript.CubeAngle == 0) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 270;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 270;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else {
					Debug.Log ("Got a wierd issue here!!");
				}
			} else if (rotations == 2) {
				if (angle == 0) { // across
					if (cubeScript.CubeAngle == 0) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 0;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else if (angle == 90) { // Down
					if (cubeScript.CubeAngle == 0) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 270;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 270;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else {
					Debug.Log ("Got a wierd issue here!!");
				}
			} else if (rotations == 3) {
				if (angle == 0) { // across
					if (cubeScript.CubeAngle == 0) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 0;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 180;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 180;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else if (angle == 90) { // Down
					if (cubeScript.CubeAngle == 0) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -180) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -90) {
						angle = 90;
					} else if (cubeScript.CubeAngle == -270) {
						angle = 90;
					} else {
						Debug.Log ("Got a wierd issue here!!");
					}
				} else {
					Debug.Log ("Got a wierd issue here!!");
				}
			}

			panelObject.transform.localEulerAngles = new Vector3 (0, angle, 0); // here needs to be set to only 2 options
			panelObject.transform.tag = ("Panel_Wall");

			break;
		case "FloorAngle":
            cubeScript._isSlope = true;
			panelObject.transform.localPosition = new Vector3 (0, 0, 0);
			panelObject.transform.localEulerAngles = new Vector3 (-135, angle, 0);
			panelObject.transform.localScale = new Vector3 (20, 30, 1);
			panelObject.transform.tag = ("Panel_FloorAngle");
			break;
		case "CeilingAngle":
            cubeScript._isSlope = true;
			panelObject.transform.localPosition = new Vector3 (0, 0, 0);
			panelObject.transform.localEulerAngles = new Vector3 (135, angle, 0);
			panelObject.transform.localScale = new Vector3 (20, 30, 1);
			panelObject.transform.tag = ("Panel_CeilingAngle");
			break;
		default:
			Debug.Log ("Got a wierd issue here!!");
			break;
		}
		panelScript.panelAngle = angle;
	}

}
