using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public class Invertor : DecoratorNode
    {
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            switch (child.result)
            {
                case Result.Success:
                    return Result.Failure;
                case Result.Failure:
                    return Result.Success;
                case Result.Running:
                    break;
            }
            return Result.Running;
        }
    }
}
