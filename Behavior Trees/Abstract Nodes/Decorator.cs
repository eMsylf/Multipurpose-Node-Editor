using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Decorator : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.ISingleConnection, NodeInterfaces.IInteractable
    {
        public Node child;

        public override Node Clone()
        {
            Decorator node = Instantiate(this);
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
    }
}