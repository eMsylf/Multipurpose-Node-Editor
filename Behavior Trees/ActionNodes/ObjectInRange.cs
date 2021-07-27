using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree.Nodes
{
    public class ObjectInRange : ActionNode
    {
        public int objectID;
        public float range = 1f;
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            GameObject obj = behaviorTreeExecutor.variableOverrides.GetGameObjectVariable(objectID);
            float distance = Vector3.Distance(behaviorTreeExecutor.transform.position, obj.transform.position);
            if (distance <= range)
            {
                return Result.Success;
            }
            return Result.Failure;
        }
    }
}