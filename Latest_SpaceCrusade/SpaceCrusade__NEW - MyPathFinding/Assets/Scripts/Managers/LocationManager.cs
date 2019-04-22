using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static LocationManager _instance;

    ////////////////////////////////////////////////
    // SERVER
    public static Dictionary<Vector3, CubeLocation_SERVER> _SERVER_LocationLookup = new Dictionary<Vector3, CubeLocation_SERVER>();   // movable locations // contains both full and half

    public static Dictionary<int, CubeLocation_SERVER> _SERVER_unitLocation = new Dictionary<int, CubeLocation_SERVER>(); // sever shit

    public static Dictionary<Vector3, BaseNode> _SERVER_nodeLookup = new Dictionary<Vector3, BaseNode>();


    ////////////////////////////////////////////////
    // CLIENT
    public static Dictionary<Vector3, CubeLocationScript> _LocationLookup = new Dictionary<Vector3, CubeLocationScript>();   // movable locations
    public static Dictionary<Vector3, CubeLocationScript> _LocationHalfLookup = new Dictionary<Vector3, CubeLocationScript>(); // not moveable locations BUT important for neighbour system

    public static Dictionary<int, CubeLocationScript> _unitLocation = new Dictionary<int, CubeLocationScript>(); // sever shit

    ////////////////////////////////////////////////

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

    ////////////////////////////////////////////////
    ////// SERVER FUNCTIONS ////////////////////////
    ////////////////////////////////////////////////

    public static void UpdateServerLocation_SERVER(Vector3 vect, bool[] cubeData)
    {
        //Debug.Log("UpdateServerLocation_SERVER");

        if (!_SERVER_LocationLookup.ContainsKey(vect))
        {
            CubeLocation_SERVER cubeServerScript = ScriptableObject.CreateInstance<CubeLocation_SERVER>();
            cubeServerScript.SetCubeData(cubeData);
            _SERVER_LocationLookup.Add(vect, cubeServerScript);
            Debug.Log("UpdateServerLocation_SERVER vect " + vect);
            Debug.Log("UpdateServerLocation_SERVER cubeServerScript.IS_ALIEN_MOVABLE " + cubeServerScript.IS_ALIEN_MOVABLE);
        }
        else
        {
            CubeLocation_SERVER cubeServerScript = _SERVER_LocationLookup[vect];
            cubeServerScript.SetCubeData(cubeData);
        }
    }

    ////////////////////////////////////////////////

    public static CubeLocation_SERVER GetLocationScript_SERVER(Vector3 loc)
    {
        //Debug.Log("GetLocationScriptSERVER");

        if (_SERVER_LocationLookup.ContainsKey(loc))
        {
            return _SERVER_LocationLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST SERVER Loc: " + loc);
        //Debug.LogError("_SERVER_LocationLookup.Count: " + _SERVER_LocationLookup.Count);
        return null;
    }

    ////////////////////////////////////////////////

    public static CubeLocation_SERVER CheckIfCanMoveToCube_SERVER(bool unitIsAlien, Vector3 loc)
    {
        //Debug.Log("CheckIfCanMoveToCube loc: " + neighloc);

        CubeLocation_SERVER cubeScript = GetLocationScript_SERVER(loc);

        if (cubeScript == null)
        {
            Debug.LogError("FAIL move cubeScript == null: " + loc);
            return null;
        }

        if (!unitIsAlien)  // if human
        {
            if (!cubeScript.IS_HUMAN_MOVABLE)
            {
                Debug.LogWarning("FAIL move error Human walkable/climable/jumpable: " + loc);
                return null;
            }
        }
        else // alien
        {
            if (!cubeScript.IS_ALIEN_MOVABLE)
            {
                Debug.LogWarning("FAIL move error ALIEN walkable/climable/jumpable: " + loc);
                return null;
            }
        }

        Debug.Log("SUCCES on SERVER move to loc: " + loc);
        return cubeScript;
    }


    public static bool SetUnitOnCube_SERVER(UnitScript unitScript, Vector3 loc)
    {
        Debug.Log("SetUnitOnCube loc:  " + loc);

        int unitNetId = (int)unitScript.NetID.Value;
        CubeLocation_SERVER cubescript = CheckIfCanMoveToCube_SERVER(unitScript, loc);
        if (cubescript != null)
        {
            if (!_SERVER_unitLocation.ContainsKey(unitNetId))
            {
                _SERVER_unitLocation.Add(unitNetId, cubescript);
            }
            else
            {
                _SERVER_unitLocation[unitNetId] = cubescript;
            }
            return true;
        }
        else
        {
            Debug.LogError("Unit cannot move to a location");
            return false;
        }
    }

    ////////////////////////////////////////////////

    public static void SetNodeScriptToLocation_SERVER(Vector3 vect, BaseNode script)
    {
        if (!_SERVER_nodeLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding normalscript to vect: " + vect + " script: " + script);
            _SERVER_nodeLookup.Add(vect, script);
        }
        else
        {
            Debug.LogError("trying to assign script to already taking location!!!");
        }
    }


    public static BaseNode GetNodeLocationScript_SERVER(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_SERVER_nodeLookup.ContainsKey(loc))
        {
            return _SERVER_nodeLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
    }

    ////////////////////////////////////////////////
    ////// CLIENT FUNCTIONS ////////////////////////
    ////////////////////////////////////////////////

    public static void SetCubeScriptToLocation_CLIENT(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding normalscript to vect: " + vect + " script: " + script);
            _LocationLookup.Add(vect, script);
            return;
        }
        Debug.LogError("trying to assign script to already taking location!!!");
    }

    public static void SetCubeScriptToHalfLocation_CLIENT(Vector3 vect, CubeLocationScript script)
    {
        if (!_LocationHalfLookup.ContainsKey(vect))
        {
            //Debug.Log("fucken adding HALF script to vect: " + vect + " script: " + script);
            _LocationHalfLookup.Add(vect, script);
            return;
        }
        Debug.LogError("trying to assign script to already taking location!!!");
    }

    ////////////////////////////////////////////////

    public static CubeLocationScript GetLocationScript_CLIENT(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationLookup.ContainsKey(loc))
        {
            return _LocationLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        //Debug.LogError("_LocationLookup.Count: " + _LocationLookup.Count);
        return null;
    }

    public static CubeLocationScript GetHalfLocationScript_CLIENT(Vector3 loc)
    {
        //Debug.Log("GetLocationScript");

        if (_LocationHalfLookup.ContainsKey(loc))
        {
            return _LocationHalfLookup[loc];
        }
        //Debug.LogError("LOCATION DOSENT EXIST Loc: " + loc);
        return null;
    }

    ////////////////////////////////////////////////
    
    
    public static void SendCubeDataToSERVER_CLIENT(Vector3 vect)
    {
        CubeLocationScript script = GetLocationScript_CLIENT(vect);
        if (script != null)
        {
            NetWorkManager.NetworkAgent.CmdTellServerToUpdateLocation(vect, script.GetCubeData());
        }
        else
        {
            Debug.LogError("GOT AN ISSUE HERE");
        }
    }
    

    public static CubeLocationScript CheckIfCanMoveToCube_CLIENT(CubeLocationScript node, Vector3 neighloc, bool unitIsAlien, bool recursiveCheck = false)
    {
        Debug.Log("CheckIfCanMoveToCube_CLIENT loc: " + neighloc);

        CubeLocationScript neighCubeScript = GetLocationScript_CLIENT(neighloc);

        if (neighCubeScript == null)
        {
            //Debug.LogError("FAIL move cubeScript == null: " + neighloc);
            return null;
        }

        neighCubeScript.IS_HUMAN_MOVABLE = false;
        neighCubeScript.IS_ALIEN_MOVABLE = false;

        if (neighCubeScript.CubeIsSlope)
        {
            //Debug.LogError("FAIL move neighCubeScript._isSlope: " + neighloc);
            return null;
        }

        if (neighCubeScript.CubeOccupied)
        {
            //Debug.LogWarning("FAIL move Cube is Occupied at vect:" + neighloc);
            return null;
        }

        // for the god damn slopes and moveing through panels (this is ugly but fuck off its working)
        if (node != null)
        {
            Vector3 nodevect = node.CubeStaticLocVector;

            // check for panel to stop going through panels
            if (node.CubeStaticLocVector.y > neighloc.y)
            {
                Vector3 neighHalf = new Vector3(nodevect.x, nodevect.y - 1, nodevect.z);
                CubeLocationScript neighscript = GetHalfLocationScript_CLIENT(neighHalf);
                if (neighscript.CubeIsPanel)
                {
                    return null;
                }
                // BOTTOM SLOPES //
                Vector3 neighSlope = new Vector3(nodevect.x, nodevect.y - 2, nodevect.z);
                neighscript = GetLocationScript_CLIENT(neighSlope);
                if (neighscript.CubeIsSlope)
                {
                    int slopeAngle = neighscript.PanelChildAngle;
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
                CubeLocationScript neighscript = GetHalfLocationScript_CLIENT(neighHalf);
                if (neighscript.CubeIsPanel)
                {
                    return null;
                }
                // TOP SLOPES // THIS WILL NEED TO BE IMPLEMENTED AT SOME POINT FOR ALIENS
                Vector3 neighSlope = new Vector3(nodevect.x, nodevect.y + 2, nodevect.z);
                neighscript = GetLocationScript_CLIENT(neighSlope);
                if (neighscript.CubeIsSlope)
                {
                    int slopeAngle = neighscript.PanelChildAngle;
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

        if (!neighCubeScript.CubePlatform)
        {
            if (recursiveCheck)
            {
                return null;
            }

            // making climbable edges ///
            if (node != null)
            {
                List<Vector3> newNeighVects = node.NeighbourVects;
                foreach (Vector3 newVect in newNeighVects)
                {
                    Debug.Log("RECURSIVE CheckIfCanMoveToCube_CLIENT loc: " + neighloc);
                    CubeLocationScript script = CheckIfCanMoveToCube_CLIENT(node, newVect, unitIsAlien, true);
                    if (script != null)
                    {
                        Debug.Log("SUCCES move to loc: " + newVect);
                        return neighCubeScript; // if climable neighboursing node return true
                    }
                }
            }
            ///////

            //Debug.LogWarning("FAIL move cubeScript not CubeMoveable: " + neighloc);
            return null;
        }


        if (!unitIsAlien)  // if human
        {
            if (neighCubeScript.IsHumanWalkable == false && neighCubeScript.IsHumanClimbable == false && neighCubeScript.IsHumanJumpable == false)
            {
                //Debug.LogWarning("FAIL move error Human walkable/climable/jumpable: " + neighloc);
                //Debug.LogFormat("FAIL walkable/climable/jumpable: {0} , {1} , {2}", neighCubeScript.IsHumanWalkable, neighCubeScript.IsHumanClimbable, neighCubeScript.IsHumanJumpable);
                return null;
            }
            neighCubeScript.IS_HUMAN_MOVABLE = true;
        }
        else if (unitIsAlien) // alien
        {
            if (neighCubeScript.IsAlienWalkable == false && neighCubeScript.IsAlienClimbable == false && neighCubeScript.IsAlienJumpable == false)
            {
                //Debug.LogWarning("FAIL move error ALIEN walkable/climable/jumpable: " + neighloc);
                return null;
            }
            neighCubeScript.IS_ALIEN_MOVABLE = true;
        }

        Debug.Log("SUCCES move to loc: " + neighloc);
        Debug.Log("SUCCES neighCubeScript.IS_ALIEN_MOVABLE: " + neighCubeScript.IS_ALIEN_MOVABLE);
        return neighCubeScript;
    }


    public static bool SetUnitOnCube_CLIENT(UnitScript unitScript, Vector3 loc)
    {
        Debug.Log("SetUnitOnCube loc:  " + loc);

        int unitNetId = (int)unitScript.NetID.Value;
        CubeLocationScript cubescript = CheckIfCanMoveToCube_CLIENT(unitScript.CubeUnitIsOn, loc, unitScript.UnitCanClimbWalls);
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
    public static void SetCubeActive_CLIENT(bool onOff, Vector3 location)
    {
        if (_activeCube)
        {
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(false);
            _activeCube = null;
        }

        if (onOff)
        {
            _activeCube = GetLocationScript_CLIENT(location);
            _activeCube.GetComponent<CubeLocationScript>().CubeActive(true);
            UnitsManager.MakeActiveUnitMove_CLIENT(location);
        }
    }

    // tries to spawn visual nodes in all current moveable locations for a unit
    public static void DebugTestPathFindingNodes_CLIENT(UnitScript unitScript)
    {
        foreach (KeyValuePair<Vector3, CubeLocationScript> element in _LocationLookup)
        {
            Debug.Log("DebugTestPathFindingNodes_CLIENT ");
            CubeLocationScript script = CheckIfCanMoveToCube_CLIENT(unitScript.CubeUnitIsOn, element.Key, unitScript.UnitCanClimbWalls);

            if (script != null)
            {
                if (script.CubeMoveable)
                {
                    script.CreatePathFindingNodeInCube((int)unitScript.NetID.Value);
                }
            }
        }
    }
}
