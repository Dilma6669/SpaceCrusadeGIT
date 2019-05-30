using UnityEngine;

public class OuterZoneBuilder : MonoBehaviour
{
    ////////////////////////////////////////////////

    private static OuterZoneBuilder _instance;

    ////////////////////////////////////////////////

    public GameObject outerZonePrefab;

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

        if (outerZonePrefab == null) { Debug.LogError("OOPSALA we have an ERROR!"); }
    }

    ////////////////////////////////////////////////
    ////////////////////////////////////////////////

    public void CreateOuterZoneForNode(WorldNode node)
    {
        Vector3 centalVect = node.NodeStaticLocation;

        BuildRestOfOuterZones(centalVect);
    }

    public void BuildRestOfOuterZones(Vector3 centalVect)
    {
        int spreadDistanceX = 10;
        int spreadDistanceY = 10;

        int multiplierX = MapSettings.sizeOfMapPiecesXZ * 3; // 3 is size of world nodes 3 is max at mo
        int multiplierY = ((MapSettings.sizeOfMapPiecesY + MapSettings.sizeOfMapVentsY) * 3); // 1 is size of world nodes 3 is max at mo

        int startX = (int)centalVect.x - (spreadDistanceX* multiplierX) - 1; // -1 to make line up properly (not sure exactly)

        int currX = startX;
        int currZ = (int)centalVect.z - 1; // -1 to make line up properly (not sure exactly)
        int currY = (int)centalVect.y - (spreadDistanceY * multiplierY);

        for (int y = 0; y < spreadDistanceY*2; y++)
        {
            for (int x = 0; x < spreadDistanceX*2; x++)
            {
                Vector3 vect = new Vector3(currX, currY, currZ);

                GameObject outerZoneObject = Instantiate(outerZonePrefab, this.transform, false); // empty cube
                outerZoneObject.transform.SetParent(this.transform);
                outerZoneObject.transform.position = vect;
                outerZoneObject.transform.localScale = new Vector3(multiplierX, multiplierY, multiplierX);

                currX += multiplierX;
            }
            currX = startX;
            currY += multiplierY;
        }
    }
}
