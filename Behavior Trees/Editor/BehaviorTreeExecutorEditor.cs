using BobJeltes.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    [CustomEditor(typeof(BehaviorTreeExecutor))]
    public class BehaviorTreeExecutorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Open editor"))
            {
                BehaviorTreeExecutor treeExecutor = (BehaviorTreeExecutor)target;
                if (treeExecutor.tree == null)
                {
                    treeExecutor.tree = CreateInstance<BehaviorTree>();
                    treeExecutor.tree.SaveTo(null, "Behavior Tree");
                }
                BehaviorTreeEditorView.OpenBehaviorTreeWindow().Load(treeExecutor.tree);
            }
        }
    }
}