using UnityEngine;

public class PathFindingNode : MonoBehaviour
{

    int _unitControllerID;
    Vector3 _cubeParentLoc;

    public int UnitControllerID
    {
        get { return _unitControllerID; }
        set { _unitControllerID = value; }
    }

    public Vector3 CubeParentLoc
    {
        get { return _cubeParentLoc; }
        set { _cubeParentLoc = value; }
    }


    private void OnTriggerEnter(Collider other)
    {
        UnitScript unitScript = other.GetComponent<UnitScript>();
        if (unitScript != null)
        {
            int unitID = (int)other.GetComponent<UnitScript>().NetID.Value;
            if (unitID == UnitControllerID)
            {
                unitScript.SetUnitToNextLocation_CLIENT();
                DestroyNode();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

    }

    public void DestroyNode()
    {
        Destroy(gameObject);
    }
}
