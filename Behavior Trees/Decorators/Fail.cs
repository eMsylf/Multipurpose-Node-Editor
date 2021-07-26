using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree.Nodes
{
    public class Fail : DecoratorNode
    {
        public override void OnStart()
        {

        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            return Result.Failure;
        }
    }
}