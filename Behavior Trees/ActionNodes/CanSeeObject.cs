using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class CanSeeObject : ActionNode
    {
        public int objectID;

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            GameObject obj = behaviorTreeExecutor.blackboardInstanceCopy.GetGameObjectVariable(objectID);
            if (Physics.Linecast(behaviorTreeExecutor.transform.position, obj.transform.position))
            {
                return Result.Failure;
            }
            return Result.Success;
        }
    }
}