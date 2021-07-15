using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class BehaviorTreeExecutor : MonoBehaviour
    {
        public BehaviorTree tree;
        public Blackboard blackboard = new Blackboard();

        private void OnValidate()
        {
            if (tree == null) blackboard = new Blackboard();
            else blackboard = tree.blackboard;
            UnityEngine.Debug.Log("Validate");
        }

        void Start()
        {
            tree = tree.Clone();
        }

        void Update()
        {
            tree?.Update();
        }

        [ContextMenu("Open behavior tree editor")]
        public void OpenBehaviorTreeEditor()
        {
            UnityEngine.Debug.Log("Open behavior tree editor");
        }
    }
}