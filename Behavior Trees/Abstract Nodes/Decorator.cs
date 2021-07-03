using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Decorator : Node, NodeInterfaces.IHasInPort, NodeInterfaces.IHasOutPort, NodeInterfaces.ISingleConnection, NodeInterfaces.IInteractable
    {
        private Node child;

        public Node Child { get => child; set => child = value; }

        public override Node Clone()
        {
            Decorator node = Instantiate(this);
            node.Child = Child?.Clone();
            return node;
        }

        public Node GetChild()
        {
            return Child;
        }

        public void SetChild(Node node)
        {
            Child = node;
        }
    }
}