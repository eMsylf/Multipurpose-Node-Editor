using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.AI.BehaviorTrees.Nodes;
using UnityEngine.Events;

public class SpawnObject : ActionNode
{
    public int prefabID;
    public bool spawnAsChild;
    public bool storeInVariable;
    [Tooltip("Object that is spawned that will be overridden in the behavior tree")]
    public int spawnedObjectID;
    public UnityEvent<GameObject> onSpawnObject = new UnityEvent<GameObject>();

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
        GameObject prefab = behaviorTreeExecutor.GetGameObjectVariable(prefabID);
        if (prefab == null)
        {
            return Result.Failure;
        }
        GameObject spawnedObject;
        if (spawnAsChild)
            spawnedObject = Instantiate(prefab, behaviorTreeExecutor.transform);
        else
            spawnedObject = Instantiate(prefab, behaviorTreeExecutor.transform.TransformPoint(prefab.transform.position), prefab.transform.rotation);

        if (storeInVariable)
        {
            TypedVariable<GameObject> gameObjectVariableOverride = behaviorTreeExecutor.variableOverrides.GameObjects.Find(x => x.id == spawnedObjectID);
            if (gameObjectVariableOverride == null)
            {
                UnityEngine.Debug.LogWarning("Object ID " + spawnedObjectID + " not found in variable overrides of " + spawnedObjectID);
            }
            else
            {
                gameObjectVariableOverride.value = spawnedObject;
            }
        }
        return Result.Success;
    }

    // This is used to copy data when creating/saving/loading nodes. Be sure to add every variable you add to this class.
    public override void CopyData(Node source)
    {
        // Example:
        // variableName = ((SpawnObject)source).variableName;
        base.CopyData(source);
        SpawnObject typedSource = ((SpawnObject)source);
        prefabID = typedSource.prefabID;
        storeInVariable = typedSource.storeInVariable;
        spawnedObjectID = typedSource.spawnedObjectID;
        onSpawnObject = typedSource.onSpawnObject;
    }
}
