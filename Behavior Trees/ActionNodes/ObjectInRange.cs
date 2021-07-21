using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class ObjectInRange : ActionNode
    {
        public int objectID;
        public float range;
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            TypedVariable<GameObject> obj = (TypedVariable<GameObject>)behaviorTreeExecutor.blackboard.GetVariable(objectID);
            float distance = Vector3.Distance(behaviorTreeExecutor.transform.position, obj.value.transform.position);
            UnityEngine.Debug.Log(distance);
            if (range <= distance)
            {
                return Result.Success;
            }
            return Result.Failure;
        }
    }
}