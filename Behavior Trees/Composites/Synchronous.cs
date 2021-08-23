using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public class Synchronous : CompositeNode
    {
        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            i = 0;
            Result result = Result.Success;
            for (; i < children.Count; i++)
            {
                result = children[i].BTUpdate(behaviorTreeExecutor);
            }
            return result;
        }
    }
}
