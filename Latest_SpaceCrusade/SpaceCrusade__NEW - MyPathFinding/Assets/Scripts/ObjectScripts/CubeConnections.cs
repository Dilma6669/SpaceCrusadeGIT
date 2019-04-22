using UnityEngine;

public class CubeConnections : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static CubeConnections _instance;

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

    // an attempt to make cubes as platforms FROM the neighbouring cubes with panels
    // the Full cube with SLOPE panel in it will be passed into this (differnt to function underneath) --dont think they are similar, they are not
    public static void SetCubeNeighbours(CubeLocationScript cubeScript)
    {
        cubeScript.SetNeighbourVects();

        // an attempt to make all sourounding cubes of Slope into movable cubes
        foreach (Vector3 vect in cubeScript.NeighbourVects)
        {
            CubeLocationScript script = LocationManager.GetLocationScript(vect);

            if (script != null)
            {
                if (script.CubeMoveable && !script._isPanel)
                {
                    script.SetNeighbourVects();
                    script.CubePlatform = true;
                }
            }
        }
        SetUpPanelInCube(cubeScript); // this call can only be made if slope panel inside full cube
    }

    // an attempt to make cubes as platforms FROM the neighbouring cubes with panels
    // the panel half cube will be passed into this
    public static void SetCubeHalfNeighbours(CubeLocationScript cubeHalfScript)
    {
        cubeHalfScript.SetHalfNeighbourVects(); // for the annoying slope issue

        // so this is essentially going through the proper neighbour cubes around the half panel cube
        foreach (Vector3 vect in cubeHalfScript.NeighbourHalfVects)
        {
            // this will only return valid full cubes, only 2 will come thru, above and below
            CubeLocationScript cubeScript = LocationManager.GetLocationScript(vect);

            if (cubeScript != null)
            {
                if (cubeScript.CubeMoveable && !cubeScript._isPanel)
                {
                    cubeScript.CubePlatform = true;
                }
                cubeScript.SetNeighbourVects(); // for the annoying slope issue
            }                                   // NO CALL TO SETUPPANELINCUBE //IMPORTANT
        }

        SetUpPanelInCube(cubeHalfScript);
    }

    ////////////////////////////////////////////////


    // If ANY kind of wall/floor/object make neighbour cubes walkable
    public static void SetUpPanelInCube(CubeLocationScript neighbourHalfScript) {

		PanelPieceScript panelScript = neighbourHalfScript._panelScriptChild;

        switch (panelScript.name) 
		{
		case "Floor":
			SetUpFloorPanel (neighbourHalfScript, panelScript);
			break;
		case "Wall":
			SetUpWallPanel (neighbourHalfScript, panelScript);
			break;
		case "FloorAngle": // angles put in half points
			SetUpFloorAnglePanel (neighbourHalfScript, panelScript);
			break;
		case "CeilingAngle": // This is the exact same as ceilingFloor
			SetUpCeilingAnglePanel (neighbourHalfScript, panelScript);
			break;
		default:
			Debug.Log ("fuck no issue:  " + panelScript.name);
			break;
		}
	}


    private static void SetHumanCubeRules(CubeLocationScript cubeScript, bool walkable, bool climable, bool jumpable)
    {
        cubeScript.IsHumanWalkable = walkable;
        cubeScript.IsHumanClimbable = climable;
        cubeScript.IsHumanJumpable = jumpable;
    }
    private static void SetAlienCubeRules(CubeLocationScript cubeScript, bool walkable, bool climable, bool jumpable)
    {
        cubeScript.IsAlienWalkable = walkable;
        cubeScript.IsAlienClimbable = climable;
        cubeScript.IsAlienJumpable = jumpable;
    }



    private static void SetUpFloorPanel(CubeLocationScript neighbourHalfScript, PanelPieceScript panelScript) {

        Vector3 cubeHalfLoc = neighbourHalfScript.CubeStaticLocVector;

        Vector3 leftVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y - 1, cubeHalfLoc.z);
        CubeLocationScript cubeScriptLeft = LocationManager.GetLocationScript(leftVect); // underneath panel
		if (cubeScriptLeft != null) {
            panelScript.cubeScriptLeft = cubeScriptLeft;
			panelScript.cubeLeftVector = leftVect;
			panelScript.leftPosNode = new Vector3 (0, 0, 0);

            SetHumanCubeRules(cubeScriptLeft, false, false, false);
            SetAlienCubeRules(cubeScriptLeft, true, true, true);
		}


        Vector3 rightVect = new Vector3(cubeHalfLoc.x, cubeHalfLoc.y + 1, cubeHalfLoc.z);
        CubeLocationScript cubeScriptRight = LocationManager.GetLocationScript(rightVect); // Ontop of panel
        if (cubeScriptRight != null)
        {
            panelScript.cubeScriptRight = cubeScriptRight;
            panelScript.cubeRightVector = rightVect;
            panelScript.rightPosNode = new Vector3(0, 0, 0);

            SetHumanCubeRules(cubeScriptRight, true, true, true);
            SetAlienCubeRules(cubeScriptRight, true, true, true);
        }

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
	}


	private static void SetUpWallPanel(CubeLocationScript neighbourHalfScript, PanelPieceScript panelScript) {

        CubeLocationScript cubeScriptLeft = null;
        CubeLocationScript cubeScriptRight = null;

        Vector3 cubeHalfLoc = neighbourHalfScript.CubeStaticLocVector;

        int cubeAngle = neighbourHalfScript.CubeAngle;
		int panelAngle = panelScript.panelAngle;

		//panelScript._isLadder = true;

		int result = (cubeAngle - panelAngle);
		result = (((result + 180) % 360 + 360) % 360) - 180;
		//Debug.Log ("cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " result: " + result);

		if (result == 180 || result == -180 || result == 0) { // Down
            Vector3 leftVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y, cubeHalfLoc.z - 1);
            cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (0, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptLeft, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptLeft, false, false, false);
                }

                SetAlienCubeRules(cubeScriptLeft, true, true, true);
            }

            Vector3 rightVect = new Vector3 (cubeHalfLoc.x, cubeHalfLoc.y, cubeHalfLoc.z + 1);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (0, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptRight, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptRight, false, false, false);
                }

                SetAlienCubeRules(cubeScriptRight, true, true, true);
            }

		} else if (result == 90 || result == -90) { //across 

            Vector3 leftVect = new Vector3 (cubeHalfLoc.x - 1, cubeHalfLoc.y, cubeHalfLoc.z);
			cubeScriptLeft = LocationManager.GetLocationScript(leftVect);
			if (cubeScriptLeft != null) {
				panelScript.cubeScriptLeft = cubeScriptLeft;
				panelScript.cubeLeftVector = leftVect;
				panelScript.leftPosNode = new Vector3 (0, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptLeft, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptLeft, false, false, false);
                }

                SetAlienCubeRules(cubeScriptLeft, true, true, true);
            }

            Vector3 rightVect = new Vector3 (cubeHalfLoc.x + 1, cubeHalfLoc.y, cubeHalfLoc.z);
			cubeScriptRight = LocationManager.GetLocationScript(rightVect);
			if (cubeScriptRight != null) {
				panelScript.cubeScriptRight = cubeScriptRight;
				panelScript.cubeRightVector = rightVect;
				panelScript.rightPosNode = new Vector3 (0, 0, 0);

                if (panelScript._isLadder)
                {
                    SetHumanCubeRules(cubeScriptRight, true, true, true);
                }
                else
                {
                    SetHumanCubeRules(cubeScriptRight, false, false, false);
                }

                SetAlienCubeRules(cubeScriptRight, true, true, true);
            }
        } else {
			Debug.Log ("SOMETHING weird: cubeAngle: " + cubeAngle + " panelAngle: " + panelAngle + " <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
		}

		if (cubeScriptLeft == null) {
			panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
			panelScript.cubeLeftVector = panelScript.cubeRightVector;
			panelScript.leftPosNode = panelScript.rightPosNode;
		}
		if (cubeScriptRight == null) {
			panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
			panelScript.cubeRightVector = panelScript.cubeLeftVector;
			panelScript.rightPosNode = panelScript.leftPosNode; 
		}
	}


    // this is a bit different, the actual MOVEABLE cube script gets passed in here coz slopes sit in the cube object not the half cube objects
    // THIS IS GOING TO CAUSE PROBLEMS IN FUTURE COZ THERES NO CHECKS IF THEY CAN MOVE ONTO SLOPE, ITS ALWAYS YES
    private static void SetUpFloorAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript)
    { 
        Vector3 cubeLoc = cubeScript.CubeStaticLocVector;

        Vector3 rightVect = new Vector3(cubeLoc.x, cubeLoc.y + 2, cubeLoc.z); // OnTop ( I think)
        CubeLocationScript cubeScriptRight = LocationManager.GetLocationScript(rightVect);
        if (cubeScriptRight != null)
        {
            panelScript.cubeScriptRight = cubeScriptRight;
            panelScript.cubeRightVector = rightVect;
            panelScript.rightPosNode = new Vector3(0, 4.5f, 0); // future issue here if want to move unit bit further on x,z axis on slope

            SetHumanCubeRules(cubeScriptRight, true, true, true);
            SetAlienCubeRules(cubeScriptRight, true, true, true);
        }

        Vector3 leftVect = new Vector3(cubeLoc.x, cubeLoc.y - 2, cubeLoc.z); // Underneath ( I think)
        CubeLocationScript cubeScriptLeft = LocationManager.GetLocationScript(leftVect);

        if (cubeScriptLeft != null)
        {
            panelScript.cubeScriptLeft = cubeScriptLeft;
            panelScript.cubeLeftVector = leftVect;
            panelScript.leftPosNode = new Vector3(0, -4.5f, 0); // future issue here if want to move unit bit further on x,z axis on slope

            SetHumanCubeRules(cubeScriptLeft, true, true, true);
            SetAlienCubeRules(cubeScriptLeft, true, true, true);
        }

        if (cubeScriptLeft == null)
        {
            panelScript.cubeScriptLeft = panelScript.cubeScriptRight;
            panelScript.cubeLeftVector = panelScript.cubeRightVector;
            panelScript.leftPosNode = panelScript.rightPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
        if (cubeScriptRight == null)
        {
            panelScript.cubeScriptRight = panelScript.cubeScriptLeft;
            panelScript.cubeRightVector = panelScript.cubeLeftVector;
            panelScript.rightPosNode = panelScript.leftPosNode;
            //Debug.LogWarning("cubeScript == null so making neighbours same cube");
        }
    }

    // this is a bit different, the actual MOVEABLE cube script gets passed in here coz slopes sit in the cube object not the half cube objects
    // THIS IS GOING TO CAUSE PROBLEMS IN FUTURE COZ THERES NO CHECKS IF THEY CAN MOVE ONTO SLOPE, ITS ALWAYS YES
    private static void SetUpCeilingAnglePanel(CubeLocationScript cubeScript, PanelPieceScript panelScript)
    {
        Vector3 cubeLoc = cubeScript.CubeStaticLocVector;

        Vector3 TopHalfVect = new Vector3(cubeLoc.x, cubeLoc.y + 1, cubeLoc.z);
        Vector3 bottomHalfVect = new Vector3(cubeLoc.x, cubeLoc.y - 1, cubeLoc.z);
        CubeLocationScript cubeScriptHalfTop = LocationManager.GetHalfLocationScript(TopHalfVect); // ontop panel
        CubeLocationScript cubeScriptHalfBottom = LocationManager.GetHalfLocationScript(bottomHalfVect); // underneath panel

        if (cubeScriptHalfBottom != null) // ontop
        {
            panelScript.cubeScriptRight = cubeScript;
            panelScript.cubeRightVector = cubeLoc;
            panelScript.rightPosNode = new Vector3(0, 4.5f, 0); // future issue here if want to move unit bit further on x,z axis on slope

            SetHumanCubeRules(cubeScript, true, true, true);
            SetAlienCubeRules(cubeScript, true, true, true);
        }

        if (cubeScriptHalfTop != null) // underneath
        {
            panelScript.cubeScriptLeft = cubeScript;
            panelScript.cubeLeftVector = cubeLoc;
            panelScript.leftPosNode = new Vector3(0, -4.5f, 0); // future issue here if want to move unit bit further on x,z axis on slope

            SetHumanCubeRules(cubeScript, true, true, true);
            SetAlienCubeRules(cubeScript, true, true, true);
        }
    }
}
