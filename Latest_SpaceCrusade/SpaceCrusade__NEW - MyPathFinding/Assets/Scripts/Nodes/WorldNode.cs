using System.Collections.Generic;

public class WorldNode : BaseNode
{
    public List<MapNode> mapNodes;
    public List<ConnectorNode> connectorNodes;


    public int worldNodeCount;

    void Awake()
    {
        NodeType = NodeTypes.WorldNode;
    }

}
