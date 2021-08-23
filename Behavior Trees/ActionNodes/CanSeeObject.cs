using BobJeltes.AI.BehaviorTrees.Nodes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees
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
            GameObject obj = behaviorTreeExecutor.GetGameObjectVariable(objectID);
            if (Physics.Linecast(behaviorTreeExecutor.transform.position, obj.transform.position, out RaycastHit hit))
            {
                if (hit.collider.gameObject != obj)
                {
                    UnityEngine.Debug.DrawLine(behaviorTreeExecutor.transform.position, hit.point, Color.red);
                    return Result.Failure;
                }
            }
            UnityEngine.Debug.DrawLine(behaviorTreeExecutor.transform.position, obj.transform.position, Color.green);
            return Result.Success;
        }
    }
}
