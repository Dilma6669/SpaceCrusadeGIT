using UnityEngine;

public class PathFindingNode : MonoBehaviour
{

    int _unitControllerID;

    public int UnitControllerID
    {
        get { return _unitControllerID; }
        set { _unitControllerID = value; }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<UnitScript>() != null)
        {
            int unitID = (int)other.GetComponent<UnitScript>().NetID.Value;
            if (unitID == UnitControllerID)
            {
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
