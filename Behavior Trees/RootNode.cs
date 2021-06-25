using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class RootNode : Node, NodeInterfaces.IHasOutPort, NodeInterfaces.ISingleConnection
    {
        public Node child;
        public void SetChild(Node node)
        {
            child = node;
        }

        public override Node Clone()
        {
            RootNode node = (RootNode)MemberwiseClone();
            node.child = child?.Clone();
            return node;
        }

        public override Result Tick()
        {
            return child.Tick();
        }

        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        public Node GetChild()
        {
            return child;
        }
    }
}
