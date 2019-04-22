using UnityEngine.EventSystems;
using UnityEngine;

public class PanelPieceScript : MonoBehaviour {

	Renderer _rend;

    public Camera _camera;

	public bool _panelActive = false;
	public bool transFlag = false;

	public int panelAngle = 0;

	public Vector3 cubeLeftVector;
	public Vector3 cubeRightVector;

	public CubeLocationScript cubeScriptParent = null;
	public CubeLocationScript cubeScriptRight = null; // Ontop (Floor)
    public CubeLocationScript cubeScriptLeft = null; // Underneath (Floor)

    private CubeLocationScript activeCubeScript = null;

	public bool _isLadder = false;

    public Vector3 leftPosNode = new Vector3();
	public Vector3 rightPosNode = new Vector3();

	//public Vector3 posActive;

	public bool cubeVisible = true;

	// Use this for initialization
	void Start () {
		_rend = GetComponent<Renderer> (); 
	//	_rend.material.color = Color.black;
	}

	void Update () {

//		if (_panelActive) {
//			PanelPieceGoTransparent ();
//			panelGoTransparent = false;
//			transFlag = true;
//		} else if (transFlag) {
//			PanelPieceGoNotTransparent ();
//			transFlag = false;
//		}
	}

	public void PanelPieceChangeColor(string color) {

		switch (color) {
		case "Red":
			_rend.material.color = Color.red;
			break;
		case "Black":
			_rend.material.color = Color.black;
			break;
		case "White":
			_rend.material.color = Color.white;
			break;
		case "Green":
			_rend.material.color = Color.green;
			break;
		default:
			break;
		}
	}


	public void ActivatePanel(bool onOff) {

		if (onOff) {
			PanelPieceChangeColor ("Red");
		} else {
			PanelPieceChangeColor ("White");
		}
		_panelActive = onOff;
	}


    void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            if (Physics.Raycast(_camera.ScreenPointToRay(Input.mousePosition), out hit))
            {
                int triIndex = hit.triangleIndex;

                Debug.Log("Hit Triangle index : " + hit.triangleIndex);

                if (triIndex == 0 || triIndex == 1 || // (if floor) To sit OnTop of panels
                        triIndex == 4 || triIndex == 5 ||
                        triIndex == 8 || triIndex == 9)
                {
                    activeCubeScript = cubeScriptRight;
                    //posActive = transform.TransformPoint(rightPosNode);
                }
                else if (triIndex == 2 || triIndex == 3 || // (if floor) To sit Underneath of panels
                        triIndex == 6 || triIndex == 7 ||
                        triIndex == 10 || triIndex == 11)
                {
                    activeCubeScript = cubeScriptLeft;
                    //posActive = transform.TransformPoint(leftPosNode);
                }
                else
                {
                    activeCubeScript = null;
                    //Debug.Log("Hit Triangle index NOT REGISTERED: " + triIndex);
                }
            }
            if (cubeScriptLeft.CubeIsVisible || cubeScriptRight.CubeIsVisible)
            {
                if (!_panelActive)
                {
                    activeCubeScript.CubeSelect(true, this.gameObject); // needs to stay here or will cause stack overflow
                    ActivatePanel(true);
                }
                else
                {
                    activeCubeScript.CubeSelect(false); // needs to stay here or will cause stack overflow
                    ActivatePanel(false);
                }
            }
        }
    }



    void OnMouseOver()
    {
        /*
		if (cubeScriptLeft == null) {
			Debug.Log ("ERROR cubeScriptLeft == null: " + this.gameObject.name);
		}
		if (cubeScriptRight == null) {
			Debug.Log ("ERROR cubeScriptRight == null" + this.gameObject.name);
		}
		if (cubeScriptLeft.cubeVisible == null) {
			Debug.Log ("ERROR cubeScriptLeft.cubeVisible == null" + this.gameObject.name);
		}
		if (cubeScriptRight.cubeVisible == null) {
			Debug.Log ("ERROR cubeScriptRight.cubeVisible == null" + this.gameObject.name);
		}
		if (cubeScriptLeft.cubeVisible || cubeScriptRight.cubeVisible) {
			if (!_panelActive) {
				PanelPieceChangeColor ("Green");
			}
		}
        */

        if (!_panelActive)
        {
            PanelPieceChangeColor("Green");
        }
    }

    void OnMouseExit()
    {
        /*
		if (cubeScriptLeft.cubeVisible || cubeScriptRight.cubeVisible) {
			if (!_panelActive) {
				PanelPieceChangeColor ("White");
			}
		}
        */
        if (!_panelActive)
        {
            PanelPieceChangeColor("White");
        }
    }

	public void PanelPieceGoTransparent() {

		if (_rend) {
			_rend.material.shader = Shader.Find ("Transparent/Diffuse");
			Color tempColor = _rend.material.color;
			tempColor.a = 0.3F;
			_rend.material.color = tempColor;
		}
	}

	public void PanelPieceGoNotTransparent() {

		if (_rend) {
			_rend.material.shader = Shader.Find ("Standard");
			Color tempColor = _rend.material.color;
			tempColor.a = 1F;
			_rend.material.color = tempColor;
		}
	}


}
