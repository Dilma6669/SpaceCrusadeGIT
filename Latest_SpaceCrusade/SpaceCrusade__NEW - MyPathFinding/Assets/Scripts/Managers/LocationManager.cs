using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LocationManager _instance;

    ////////////////////////////////////////////////

    public static Dictionary<Vector3, BaseNode> _nodeLookup = new Dictionary<Vector3, BaseNode>(); // not moveable locations BUT important for neighbour system

    public static Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();   // movable locations
    public static Dictionary<Vector3, CubeLocationScript> _LocationHalfLookup = new Dictionary<Vector3, CubeLocationScript>(); // not moveable locations BUT important for neighbour system

    public static Dictionary<int, CubeLocationScript> _unitLocation = new Dictionary<int, CubeLocationScript>(); // sever shit

    private static CubeLocationScript _activeCube = null; // hmmm dont know if should be here

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

    public static void SetNodeScriptToLocation(Vector3 vect, BaseNode script)
    {
        if (!_nodeLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding normalscript to vect: " + vect + " script: " + script);
            _nodeLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }

    public static BaseNode GetNodeLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_nodeLookup.ContainsKey(loc))
        {
            return _nodeLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
    }

    ////////////////////////////////////////////////

    public static void SetCubeScriptToLocation(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding normalscript to vect: " + vect + " script: " + script);
            _LocationLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }

    public static void SetCubeScriptToHalfLocation(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationHalfLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding HALF script to vect: " + vect + " script: " + script);
            _LocationHalfLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }

    ////////////////////////////////////////////////

    public static CubeLocationScript GetLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationLookup.ContainsKey(loc)) {
            return _LocationLookup[loc];
		}
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
	}

    public static CubeLocationScript GetHalfLocationScript(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationHalfLookup.ContainsKey(loc))
        {
            return _LocationHalfLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
    }


    public static CubeLocationScript CheckIfCanMoveToCube(UnitScript unit, CubeLocationScript node, Vector3 neighloc, bool recursiveCheck = false)
    {
        //Debug.Log("CheckIfCanMoveToCube loc: " + neighloc);

        CubeLocationScript neighCubeScript = GetLocationScript(neighloc);

        if (neighCubeScript == null)
        {
            //Debug.LogError("FAIL move cubeScript == null: " + neighloc);
            return null;
        }

        if (neighCubeScript._isSlope)
        {
            // Debug.LogError("FAIL move neighCubeScript._isSlope: " + neighloc);
            return null;
        }

        if (!neighCubeScript.CubePlatform)
        {
            if(recursiveCheck)
            {
                return null;
            }

            // making climbable edges ///
            List<Vector3> newNeighVects = node.NeighbourVects;
            foreach (Vector3 newVect in newNeighVects)
            {
                CubeLocationScript script = CheckIfCanMoveToCube(unit, node, newVect, true);
                if(script != null)
                {
                    return neighCubeScript; // if climable neighboursing node return true
                }
            }
            ///////

            //Debug.LogWarning("FAIL move cubeScript not CubeMoveable: " + neighloc);
            return null;
        }

        if (neighCubeScript.CubeOccupied)
        {
            //Debug.LogWarning("FAIL move Cube is Occupied at vect:" + neighloc);
            return null;
        }

        if (!unit.UnitCanClimbWalls)  // if human
        {
            if (neighCubeScript.IsHumanWalkable == false && neighCubeScript.IsHumanClimbable == false && neighCubeScript.IsHumanJumpable == false)
            {
               // Debug.LogWarning("FAIL move error Human walkable/climable/jumpable: " + neighloc);
                //Debug.LogFormat("FAIL walkable/climable/jumpable: {0} , {1} , {2}", neighCubeScript.IsHumanWalkable, neighCubeScript.IsHumanClimbable, neighCubeScript.IsHumanJumpable);
                return null;
            }
        }
        else // alien
        {
            if (neighCubeScript.IsAlienWalkable == false && neighCubeScript.IsAlienClimbable == false && neighCubeScript.IsAlienJumpable == false)
            {
                //Debug.LogWarning("FAIL move error ALIEN walkable/climable/jumpable: " + neighloc);
                return null;
            }
        }

        // for the god damn slopes and moveing through panels (this is ugly but fuck off its working)
        if (node != null)
        {
            Vector3 nodevect = node.CubeStaticLocVector;

            // check for panel to stop going through panels
            if (node.CubeStaticLocVector.y > neighloc.y)
            {
                Vector3 neighHalf = new Vector3(nodevect.x, nodevect.y - 1, nodevect.z);
                CubeLocationScript neighscript = GetHalfLocationScript(neighHalf);
                if (neighscript._isPanel)
                {
                    return null;
                }
                // BOTTOM SLOPES //
                Vector3 neighSlope = new Vector3(nodevect.x, nodevect.y - 2, nodevect.z);
                neighscript = GetLocationScript(neighSlope);
                if (neighscript._isSlope)
                {
                    int slopeAngle = neighscript._panelScriptChild.panelAngle;
                    if (slopeAngle == 90)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[3];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                    else if (slopeAngle == 180)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[0];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                    else if (slopeAngle == 270)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[1];
                        if (neighloc == neighBehindVect) { return null; }

                    }
                    else if (slopeAngle == 0)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[4];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                }
            }
            else
            {
                Vector3 neighHalf = new Vector3(nodevect.x, nodevect.y + 1, nodevect.z);
                CubeLocationScript neighscript = GetHalfLocationScript(neighHalf);
                if (neighscript._isPanel)
                {
                    return null;
                }
                // TOP SLOPES // THIS WILL NEED TO BE IMPLEMENTED AT SOME POINT FOR ALIENS
                Vector3 neighSlope = new Vector3(nodevect.x, nodevect.y + 2, nodevect.z);
                neighscript = GetLocationScript(neighSlope);
                if (neighscript._isSlope)
                {
                    int slopeAngle = neighscript._panelScriptChild.panelAngle;
                    if (slopeAngle == 90)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[3];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                    else if (slopeAngle == 180)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[0];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                    else if (slopeAngle == 270)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[1];
                        if (neighloc == neighBehindVect) { return null; }

                    }
                    else if (slopeAngle == 0)
                    {
                        Vector3 neighBehindVect = node.NeighbourVects[4];
                        if (neighloc == neighBehindVect) { return null; }
                    }
                }
            }
        }


        //Debug.Log("SUCCES move to loc: " + neighloc);

        // success
        return neighCubeScript;
    }



    public static bool SetUnitOnCube(UnitScript unitScript, Vector3 loc)
    {
        //Debug.Log("SetUnitOnCube");

        int unitNetId = (int)unitScript.NetID.Value;
        CubeLocationScript cubescript = CheckIfCanMoveToCube(unitScript, unitScript.CubeUnitIsOn, loc);
        if (cubescript != null)
        {
            if (!_unitLocation.ContainsKey(unitNetId))
            {
                _unitLocation.Add(unitNetId, cubescript);
            }
            else
            {
                CubeLocationScript oldCubescript = _unitLocation[unitNetId];
                oldCubescript.CubeOccupied = false;
                oldCubescript.FlagToSayIsMine = null;
                _unitLocation[unitNetId] = cubescript;
            }
            cubescript.CubeOccupied = true;
            unitScript.CubeUnitIsOn = cubescript;
            return true;
        }
        else
        {
            Debug.LogError("Unit cannot move to a location");
            return false;
        }
    }



    ////// Dont think this should be here
    public static void SetCubeActive(bool onOff, KeyValuePair<Vector3, Vector3> posRot)
    {
        if (_activeCube)
        {
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(false);
            _activeCube = null;
        }

        if (onOff)
        {
            _activeCube = GetLocationScript(posRot.Key);
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(true);
            UnitsManager.MakeActiveUnitMove(posRot);
        }
    }

    // tries to spawn visual nodes in all current moveable locations for a unit
    public static void DebugTestPathFindingNodes(UnitScript unit)
    {
        foreach (KeyValuePair<Vector3, CubeLocationScript> element in _LocationLookup)
        {
            CubeLocationScript script = CheckIfCanMoveToCube(unit, unit.CubeUnitIsOn, element.Key);

            if (script != null)
            {
                if (script.CubeMoveable)
                {
                    script.CreatePathFindingNode((int)unit.NetID.Value);
                }
            }
        }
    }
}
