using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public class Repeat : DecoratorNode
    {
        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            child.BTUpdate(behaviorTreeExecutor);
            return Result.Running;
        }
    }
}
