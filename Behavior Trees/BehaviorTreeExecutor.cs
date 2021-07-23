using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class BehaviorTreeExecutor : MonoBehaviour
    {
        public BehaviorTree tree;
        public Blackboard blackboardInstanceCopy;

        private void OnValidate()
        {
        }

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
            UnityEngine.Debug.Log("Open behavior tree editor");
        }

        public void RebuildInstanceOverrideList()
        {
            UnityEngine.Debug.Log("Rebuild", this);
            if (tree == null)
            {
                UnityEngine.Debug.Log("Tree is null");
                blackboardInstanceCopy = new Blackboard();
                return;
            }

            blackboardInstanceCopy = new Blackboard(tree.blackboard);
        }

        public GameObject GetGameObjectVariable(int id)
        {
            GameObject instanceOverride = blackboardInstanceCopy.GetGameObjectVariable(id);
            if (instanceOverride != null)
                return instanceOverride;
            GameObject blackboardGameObject = tree.blackboard.GetGameObjectVariable(id);
            return blackboardGameObject;
        }
    }
}