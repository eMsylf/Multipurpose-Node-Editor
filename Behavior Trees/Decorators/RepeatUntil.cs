using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;

public class RepeatUntil : DecoratorNode
{
    public enum RepeatCondition
    {
        Fail
    }
    public RepeatCondition repeatCondition = RepeatCondition.Fail;
    public bool conditionMet = false;

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
        if (conditionMet) return Result.Failure;
        Result childResult = child.BTUpdate(behaviorTreeExecutor);
        switch (repeatCondition)
        {
            case RepeatCondition.Fail:
                if (childResult == Result.Failure)
                    conditionMet = true;
                break;
            default:
                break;
        }
        return childResult;
    }
}
