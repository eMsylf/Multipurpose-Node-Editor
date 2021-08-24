using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;

public class ApplyForceToObject : ActionNode
{
    public int objectID;
    public Vector3 force = Vector3.forward;
    public ForceMode forceMode = ForceMode.Impulse;
    public enum RelativeObjectOption
    {
        None,
        Self,
        Other
    }
    public RelativeObjectOption relativeObject = RelativeObjectOption.None;
    [Tooltip("Only assign when Relative Object is set to Other")]
    public int otherObjectID;

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
        TypedVariable<GameObject> objectVariable = behaviorTreeExecutor.variableOverrides.GameObjects.Find(x => x.id == objectID);
        if (objectVariable == null)
        {
            return Result.Failure;
        }
        GameObject objectToApplyForceTo = objectVariable.value;
        if (objectToApplyForceTo == null)
        {
            UnityEngine.Debug.LogError("No object to apply a force to", behaviorTreeExecutor);
            return Result.Failure;
        }
        Rigidbody rigidbody = objectToApplyForceTo.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            UnityEngine.Debug.LogError("Provided prefab object of " + name + " in behavior tree of " + behaviorTreeExecutor.name + " has no rigidbody.");
            return Result.Failure;
        }
        
        switch (relativeObject)
        {
            default:
            case RelativeObjectOption.None:
                rigidbody.AddForce(force, forceMode);
                break;
            case RelativeObjectOption.Self:
                rigidbody.AddForce(
                behaviorTreeExecutor.transform.TransformVector(force),
                forceMode);
                break;
            case RelativeObjectOption.Other:
                rigidbody.AddForce(
                behaviorTreeExecutor.GetGameObjectVariable(otherObjectID).transform.TransformVector(force),
                forceMode);
                break;
        }
        return Result.Success;
    }
}
