namespace DEF
{
    public class NodeGraphContext
    {
        public NodeGraphContext()
        {
        }

        public void RegisterNodeGraph(NodeGraphFactroy factory)
        {
        }

        public NodeGraph GetNodeGraph(string nodegraph_name)
        {
            return new NodeGraph();
        }
    }
}