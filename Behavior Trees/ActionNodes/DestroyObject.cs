using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;

public class DestroyObject : ActionNode
{
    public int objectID;
    [Min(0)]
    public float delay = 0;

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
        GameObject gameObject = behaviorTreeExecutor.GetGameObjectVariable(objectID);
        if (gameObject == null)
        {
            return Result.Failure;
        }
        if (delay > 0)
        {
            Destroy(gameObject, delay);
        }
        else
        {
            Destroy(gameObject);
        }
        return Result.Success;
    }
}
