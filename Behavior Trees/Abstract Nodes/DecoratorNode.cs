using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public abstract class DecoratorNode : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.ISingleConnection, NodeInterfaces.IInteractable
    {
        public Node child;

        public override Node Clone()
        {
            DecoratorNode node = Instantiate(this);
            node.child = child?.Clone();
            return node;
        }

        public Node GetChild()
        {
            return child;
        }

        public void SetChild(Node node)
        {
            child = node;
        }

        public override void CopyData(Node source)
        {
            child = ((DecoratorNode)source).child;
            base.CopyData(source);
        }
    }
}