using System.Collections;
using System.Collections.Generic;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Composite : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.IMultipleConnection, NodeInterfaces.IInteractable
    {
        public List<Node> children = new List<Node>();
        public int i = 0;

        public void AddChild(Node node)
        {
            children.Add(node);
        }

        public override Node Clone()
        {
            Composite node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }

        public List<Node> GetChildren()
        {
            return children;
        }

        public void SetChildren(List<Node> nodes)
        {
            children = nodes;
        }
    }
}