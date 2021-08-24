using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;
using UnityEngine.AI;

public class NavMeshStart : ActionNode
{
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
        NavMeshAgent navMeshAgent = behaviorTreeExecutor.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            UnityEngine.Debug.LogError(behaviorTreeExecutor + " has no nav mesh agent component assigned.", behaviorTreeExecutor);
        }
        navMeshAgent.isStopped = false;
        return Result.Success;
    }
}
