using System.Collections;
using System.Collections.Generic;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Composite : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.IMultipleConnection, NodeInterfaces.IInteractable
    {
        private List<Node> children = new List<Node>();
        protected int i = 0;

        public List<Node> Children { get => children; set => children = value; }

        public void AddChild(Node node)
        {
            Children.Add(node);
        }

        public override Node Clone()
        {
            Composite node = Instantiate(this);
            node.Children = Children.ConvertAll(c => c.Clone());
            return node;
        }

        public List<Node> GetChildren()
        {
            return Children;
        }

        public void RemoveChild(Node node)
        {
            Children.Remove(node);
        }

        public void SetChildren(List<Node> nodes)
        {
            Children = nodes;
        }
    }
}