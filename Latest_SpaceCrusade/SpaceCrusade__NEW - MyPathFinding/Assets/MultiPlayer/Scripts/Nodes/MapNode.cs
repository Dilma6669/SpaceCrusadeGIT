using System.Collections.Generic;

public class MapNode : BaseNode {

    public List<int[,]> mapFloorData = new List<int[,]>();
    public List<int[,]> mapVentData = new List<int[,]>();

    void Awake()
    {
        NodeType = NodeTypes.MapNode;
    }

    public void RemoveDoorPanels()
    {
        //Debug.Log("removing door panels");
    }
}
