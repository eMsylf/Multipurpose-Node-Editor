using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Sequence : Composite
    {
        public override void OnStart()
        {
            i = 0;
        }

        public override void OnStop()
        {
            
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            for (; i < children.Count; i++)
            {
                Result result = children[i].BTUpdate(behaviorTreeExecutor);
                switch (result)
                {
                    case Result.Failure:
                        i = 0;
                        return result;
                    case Result.Running:
                        return result;
                }
            }
            i = 0;
            return Result.Success;
        }
    }
}
