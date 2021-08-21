using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;
using UnityEngine.AI;

public class SetNavMeshDestination : ActionNode
{
    public int targetID;
    private NavMeshAgent agent;

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
        
        if (agent == null)
        {
            agent = behaviorTreeExecutor.GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                UnityEngine.Debug.LogError(behaviorTreeExecutor.name + " has no NavMeshAgent component assigned but " + name + " is trying to access it.", behaviorTreeExecutor);
                return Result.Failure;
            }
        }

        GameObject target = behaviorTreeExecutor.variableOverrides.GetGameObjectVariable(targetID);
        if (target == null)
        {
            UnityEngine.Debug.LogError("Did not find target object with id " + targetID);
            return Result.Failure;
        }

        agent.SetDestination(target.transform.position);

        return Result.Success;
    }
}
