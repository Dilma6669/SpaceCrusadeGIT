
using System.Collections.Generic;
using UnityEngine;

public class ConnectorNode : BaseNode
{
    public List<int[,]> connectorFloorData = new List<int[,]>();
    public List<int[,]> connectorVentData = new List<int[,]>();





    void Awake()
    {
        NodeType = NodeTypes.ConnectorNode;
    }

    public void RemoveDoorPanels()
    {
        Debug.Log("removing door panels");
    }
}
