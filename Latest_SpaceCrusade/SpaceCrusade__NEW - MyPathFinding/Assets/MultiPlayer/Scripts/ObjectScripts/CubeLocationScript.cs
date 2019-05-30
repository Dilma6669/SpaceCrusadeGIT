using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeLocationScript : MonoBehaviour {

    // Cube info
    int _cubeUniqueID;
    Vector3 _cubeStaticLoc;
    int _cubeType;
    int _cubeAngle;
    int _cubeLayerID;
    bool _cubeMovable; // this is all movable cubes everywhere, including in the air
    bool _cubePlatform; // this is all movable cubes only with panels to walk on, not in air
    bool _cubeIsSlope;
    bool _cubeIsPanel;

    bool _cubeVisible;
    bool _cubSelected;
    bool _cubeOccupied; // If a guy is on square

    bool _isHumanWalkable;
    bool _isHumanClimbable;
    bool _isHumanJumpable;
    bool _isAlienWalkable;
    bool _isAlienClimbable;
    bool _isAlienJumpable;

    // All Checks combined into two bools
    bool _isHumanMoveable;
    bool _isAlienMoveable;

    // panel objects
    int _panelChildAngle;
    public GameObject _activePanel;
    public PanelPieceScript _panelScriptChild = null;

    // Pathfinding
    public GameObject _pathFindingNode;
    public CubeLocationScript _parentPathFinding; //important
    public int fCost;
    public int hCost;
    public int gCost;

    // neighbour Cubes
    public List<Vector3> _neighVects = new List<Vector3>();
    public List<Vector3> _neighHalfVects = new List<Vector3>();
    public bool _neighboursSet = false;
    bool[] _neighBools = new bool[27];


    public int CubeUniqueID
    {
        get { return _cubeUniqueID; }
        set { _cubeUniqueID = value; }
    }

    public Vector3 CubeStaticLocVector
    {
        get { return _cubeStaticLoc; }
        set { _cubeStaticLoc = value; }
    }

    public bool CubeMoveable
    {
        get { return _cubeMovable; }
        set { _cubeMovable = value; }
    }

    public bool CubePlatform
    {
        get { return _cubePlatform; }
        set { _cubePlatform = value; }
    }

    public int CubeType
    {
        get { return _cubeType; }
        set { _cubeType = value; }
    }

    public int CubeAngle
    {
        get { return _cubeAngle; }
        set { _cubeAngle = value; }
    }

    public bool CubeIsVisible
    {
        get { return _cubeVisible; }
        set { _cubeVisible = value; }
    }

    public bool CubeSelected
    {
        get { return _cubSelected; }
        set { _cubSelected = value; }
    }

    public bool CubeOccupied
    {
        get { return _cubeOccupied; }
        set { _cubeOccupied = value; }
    }

    public bool CubeIsSlope
    {
        get { return _cubeIsSlope; }
        set { _cubeIsSlope = value; }
    }

    public bool CubeIsPanel
    {
        get { return _cubeIsPanel; }
        set { _cubeIsPanel = value; }
    }


    public int CubeLayerID
    {
        get { return _cubeLayerID; }
        set { _cubeLayerID = value; }
    }

    // Human
    public bool IsHumanWalkable
    {
        get { return _isHumanWalkable; }
        set { _isHumanWalkable = value; }
    }
    public bool IsHumanClimbable
    {
        get { return _isHumanClimbable; }
        set { _isHumanClimbable = value; }
    }
    public bool IsHumanJumpable
    {
        get { return _isHumanJumpable; }
        set { _isHumanJumpable = value; }
    }
    // Alien
    public bool IsAlienWalkable
    {
        get { return _isAlienWalkable; }
        set { _isAlienWalkable = value; }
    }
    public bool IsAlienClimbable
    {
        get { return _isAlienClimbable; }
        set { _isAlienClimbable = value; }
    }
    public bool IsAlienJumpable
    {
        get { return _isAlienJumpable; }
        set { _isAlienJumpable = value; }
    }

    public bool IS_HUMAN_MOVABLE
    {
        get { return _isHumanMoveable; }
        set { _isHumanMoveable = value; }
    }
    public bool IS_ALIEN_MOVABLE
    {
        get { return _isAlienMoveable; }
        set { _isAlienMoveable = value; }
    }



    public int PanelChildAngle
    {
        get { return _panelChildAngle; }
        set { _panelChildAngle = value; }
    }


    // Neighbours
    public bool NeighboursSet
    {
        get { return _neighboursSet; }
        set { _neighboursSet = value; }
    }

    public List<Vector3> NeighbourVects
    {
        get { return _neighVects; }
        set { _neighVects = value; }
    }

    public List<Vector3> NeighbourHalfVects
    {
        get { return _neighHalfVects; }
        set { _neighHalfVects = value; }
    }


    void Awake()
    {
        CubeIsVisible = true;
        CubeSelected = false;
        CubeOccupied = false;
        CubeMoveable = false;
        CubePlatform = false;
        CubeIsSlope = false;

        IsHumanWalkable = false;
        IsHumanClimbable = false;
        IsHumanJumpable = false;

        IsAlienWalkable = false;
        IsAlienClimbable = false;
        IsAlienJumpable = false;

        IS_HUMAN_MOVABLE = false;
        IS_ALIEN_MOVABLE = false;

        CubeStaticLocVector = new Vector3(-1, -1, -1);
    }

    public void AssignCubeNeighbours()
    {
        if (!NeighboursSet)
        {
            if (CubeMoveable)
            {
                CubeConnections.SetCubeNeighbours(this);
            }
            else
            {
                CubeConnections.SetCubeHalfNeighbours(this);
            }
            NeighboursSet = true;
        }
    }



    public void CubeActive(bool onOff) {
		
		if (onOff) {
			CubeSelected = true;
		} else {
            CubeSelected = false;
			_activePanel.GetComponent<PanelPieceScript> ().ActivatePanel (false);
		}
	}

	///////////////////////////////
	/// this is for when panel is clicked
	public void CubeSelect(bool onOff, GameObject panelSelected = null) {

		if (onOff) {
			CubeActive (true);
			_activePanel = panelSelected;
            LocationManager.SetCubeActive_CLIENT(true, CubeStaticLocVector); // not sure if this should be here yet
        }
        else
        {
			CubeActive (false);
            LocationManager.SetCubeActive_CLIENT(false, Vector3.zero);
		}
	}


    public void SetHalfNeighbourVects() {
        
        if(!NeighbourHalfVects.Any())
        {
            Vector3 ownVect = new Vector3(CubeStaticLocVector.x, CubeStaticLocVector.y, CubeStaticLocVector.z);
    
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z - 1)); // 0
            //neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z - 1)); // 1
            //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z - 1)); // 2
            //
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z + 0)); // 3
            NeighbourHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z + 0)); // 4 directly below
            //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z + 0)); // 5
                                                                                            //
             //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y - 1, ownVect.z + 1)); // 6
             //neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 1, ownVect.z + 1)); // 7
             //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y - 1, ownVect.z + 1)); // 8
    
            /////////////////////////////////
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z - 1)); // 9
            NeighbourHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z - 1)); // 10 infront (south)
            //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z - 1)); // 11
    
            NeighbourHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z + 0)); // 12 side (west)
            //NeighbourHalfVects.Add(ownVect);                                                  // 13 //// MIDDLE
            NeighbourHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z + 0)); // 14 side (east)
    
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 0, ownVect.z + 1)); // 15
            NeighbourHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z + 1)); // 16 back (North)
           //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 0, ownVect.z + 1)); // 17 
                                                                                            /////////////////////////////////
    
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z - 1)); // 18
            //neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z - 1)); // 19
            //neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z - 1)); // 20
            //
            //neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z + 0)); // 21
            NeighbourHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z + 0)); // 22 directly above
    		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z + 0)); // 23
    		//
    		//neighHalfVects.Add(new Vector3 (ownVect.x - 1, ownVect.y + 1, ownVect.z + 1)); // 24
    		//neighHalfVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 1, ownVect.z + 1)); // 25
    		//neighHalfVects.Add(new Vector3 (ownVect.x + 1, ownVect.y + 1, ownVect.z + 1)); // 26

        /////////////////////////////////
        }
    }

	public void SetNeighbourVects() {

        if (!NeighbourVects.Any())
        {
            Vector3 ownVect = new Vector3(CubeStaticLocVector.x, CubeStaticLocVector.y, CubeStaticLocVector.z);
    
            //NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z - 2)); // 0
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z - 2)); // 1
            //NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z - 2)); // 2
                                                                                       //
            NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z + 0)); // 3
    		NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z + 0)); // 4 directly below
            NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z + 0)); // 5
                                                                                           //
            //NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y - 2, ownVect.z + 2)); // 6
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y - 2, ownVect.z + 2)); // 7
            //NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y - 2, ownVect.z + 2)); // 8
    
            /////////////////////////////////
            NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z - 2)); // 9
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z - 2)); // 10 infront (south)
            NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z - 2)); // 11
    
            NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z + 0)); // 12 side (west)
            //NeighbourVects.Add(ownVect);                                                   // 13 //// MIDDLE
            NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z + 0)); // 14 side (east)
    
            NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 0, ownVect.z + 2)); // 15
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 0, ownVect.z + 2)); // 16 back (North)
            NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 0, ownVect.z + 2)); // 17 
            /////////////////////////////////
    
            //NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z - 2)); // 18
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z - 2)); // 19
            //NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 2, ownVect.z - 2)); // 20
            //
            NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z + 0)); // 21
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z + 0)); // 22 directly above
            NeighbourVects.Add(new Vector3 (ownVect.x + 2, ownVect.y + 2, ownVect.z + 0)); // 23
                                                                                           //
            //NeighbourVects.Add(new Vector3 (ownVect.x - 2, ownVect.y + 2, ownVect.z + 2)); // 24
            NeighbourVects.Add(new Vector3 (ownVect.x + 0, ownVect.y + 2, ownVect.z + 2)); // 25
          
         /////////////////////////////////
        }
    }

    // data for the server
    public bool[] GetCubeData()
    {
        bool[] data = new bool[2];

        data[0] = IS_HUMAN_MOVABLE;
        data[1] = IS_ALIEN_MOVABLE;

        return data;
    }


    public void ResetPathFindingValues()
    {
        _pathFindingNode = null;
        _parentPathFinding = null;
        fCost = 0;
        hCost = 0;
        gCost = 0;
    }


	public void CreatePathFindingNodeInCube(int unitID) {
        _pathFindingNode = WorldBuilder._nodeBuilder.CreatePathFindingNode(transform, unitID);
        _pathFindingNode.transform.position = transform.position;
        _pathFindingNode.transform.localScale = new Vector3(10, 10, 10);
        _pathFindingNode.GetComponent<PathFindingNode>().CubeParentLoc = CubeStaticLocVector;
    }

    public void DestroyPathFindingNode()
    {
        if (_pathFindingNode != null)
        {
            _pathFindingNode.GetComponent<PathFindingNode>().DestroyNode();
        }
        ResetPathFindingValues();
    }

}
