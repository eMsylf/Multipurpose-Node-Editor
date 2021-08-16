using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public abstract class CompositeNode : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.IMultipleConnection, NodeInterfaces.IInteractable
    {
        public List<Node> children = new List<Node>();
        public int i = 0;

        public void AddChild(Node node)
        {
            children.Add(node);
            children = children.OrderBy(x => x.positionOnView.x).ToList();
        }

        public override Node Clone()
        {
            CompositeNode node = (CompositeNode)base.Clone();
            node.children = new List<Node>();
            foreach (var child in children)
            {
                if (child == null)
                {
                    UnityEngine.Debug.LogError("Child is null");
                    continue;
                }
                node.children.Add(child.Clone());
            }
            return node;
        }

        public List<Node> GetChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                Node item = children[i];
                if (item == null)
                {
                    UnityEngine.Debug.LogError("Child node " + i + " of " + name + " is null");
                    children.RemoveAt(i);
                    i--;
                }
            }
            return children;
        }

        public void RemoveChild(Node node)
        {
            children.Remove(node);
        }

        public void SetChildren(List<Node> nodes)
        {
            children = nodes.OrderBy(x => x.positionOnView.x).ToList();
        }

        public override void CopyData(Node source)
        {
            children = ((CompositeNode)source).children;
            base.CopyData(source);
        }
    }
}