using BobJeltes.AI.BehaviorTrees.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees
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
            UnityEngine.Debug.Log("Clone root");
            RootNode node = Instantiate(this);
            node.child = child?.Clone();
            return node;
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            return child.BTUpdate(behaviorTreeExecutor);
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

        public override void CopyData(Node source)
        {
            child = ((RootNode)source).child;
            base.CopyData(source);
        }
    }
}
