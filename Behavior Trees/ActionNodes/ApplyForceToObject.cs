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
            UnityEngine.Debug.LogError("");
            return Result.Failure;
        }
        Rigidbody rigidbody = objectToApplyForceTo.GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            UnityEngine.Debug.LogError("Provided prefab object of " + name + " in behavior tree of " + behaviorTreeExecutor.name + " has no rigidbody.");
            return Result.Failure;
        }
        rigidbody.AddForce(force, forceMode);
        return Result.Success;
    }

    // This is used to copy data when creating/saving/loading nodes. Be sure to add every variable you add to this class.
    public override void CopyData(Node source)
    {
        // Example:
        // variableName = ((ApplyForceToObject)source).variableName;
        base.CopyData(source);
        ApplyForceToObject typedSource = ((ApplyForceToObject)source);
        force = typedSource.force;
        forceMode = typedSource.forceMode;
        objectID = typedSource.objectID;
    }
}
