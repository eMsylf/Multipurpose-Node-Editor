using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class ObjectInRange : ActionNode
    {
        public GameObject Object;
        public float range;
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            float distance = Vector3.Distance(behaviorTreeExecutor.transform.position, Object.transform.position);
            UnityEngine.Debug.Log(distance);
            if (range <= distance)
            {
                return Result.Success;
            }
            return Result.Failure;
        }
    }
}