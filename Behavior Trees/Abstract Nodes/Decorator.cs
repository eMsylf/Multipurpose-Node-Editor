using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Decorator : Node
    {
        public Node child;

        public override Node Clone()
        {
            Decorator node = (Decorator)MemberwiseClone();
            node.child = child.Clone();
            return node;
        }
    }
}