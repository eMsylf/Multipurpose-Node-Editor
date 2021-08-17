using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees
{
    [AddComponentMenu("Bob Jeltes/AI/Behavior Trees/Behavior Tree Executor")]
    public class BehaviorTreeExecutor : MonoBehaviour
    {
        public BehaviorTree tree;
        public Blackboard variableOverrides;

        void Start()
        {
            tree = tree.Clone();
        }

        void Update()
        {
            tree?.BTUpdate(this);
        }

        [ContextMenu("Open behavior tree editor")]
        public void OpenBehaviorTreeEditor()
        {
            Debug.Log("Open behavior tree editor");
        }

        public void RebuildInstanceOverrideList()
        {
            if (tree == null)
            {
                Debug.Log("Tree is null while trying to rebuild the instance override blackboard of " + name, this);
                variableOverrides = new Blackboard();
                return;
            }

            variableOverrides = new Blackboard(tree.blackboard);
        }

        public GameObject GetGameObjectVariable(int id)
        {
            GameObject instanceOverride = variableOverrides.GetGameObjectVariable(id);
            if (instanceOverride != null)
                return instanceOverride;
            GameObject blackboardGameObject = tree.blackboard.GetGameObjectVariable(id);
            return blackboardGameObject;
        }
    }
}