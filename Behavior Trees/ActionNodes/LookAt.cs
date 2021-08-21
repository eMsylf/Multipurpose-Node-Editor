using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;

public class LookAt : ActionNode
{
    public int objectID;
    public bool stayUpright;

    // This is called when the node is first updated
    public override void OnStart()
    {
	    
    }

    // This is called when the node stops being updated
    public override void OnStop()
    {
	    
    }

    // This is called when the node is updated
    public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
    {
        GameObject obj = behaviorTreeExecutor.GetGameObjectVariable(objectID);
        if (obj == null)
        {
            return Result.Failure;
        }

        Vector3 lookTarget = obj.transform.position;

        if (stayUpright) 
            lookTarget.y = behaviorTreeExecutor.transform.position.y;

        behaviorTreeExecutor.transform.LookAt(lookTarget);
        return Result.Success;
    }
}
