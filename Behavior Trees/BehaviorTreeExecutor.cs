using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class BehaviorTreeExecutor : MonoBehaviour
    {
        public BehaviorTree tree;

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