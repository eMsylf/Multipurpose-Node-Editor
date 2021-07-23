using BobJeltes.AI.BehaviorTree;
using System;
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
            DrawPropertiesExcluding(serializedObject, "script", "tree", "scene objects");
            BehaviorTreeExecutor treeExecutor = (BehaviorTreeExecutor)target;
            
            // Check if the object reference was changed
            EditorGUI.BeginChangeCheck();
            BehaviorTree oldTree = treeExecutor.tree;
            treeExecutor.tree = (BehaviorTree)EditorGUILayout.ObjectField("Behavior Tree", treeExecutor.tree, typeof(BehaviorTree), false);
            if (EditorGUI.EndChangeCheck())
            {
                if (oldTree != null)
                {
                    // Tree reference changed, unsubscribe from old tree
                    oldTree.OnBehaviorTreeAltered -= () => treeExecutor.RebuildInstanceOverrideList();
                }
                if (treeExecutor.tree != null)
                {
                    // Has new tree assigned
                    treeExecutor.tree.OnBehaviorTreeAltered += () => treeExecutor.RebuildInstanceOverrideList();
                }
                treeExecutor.RebuildInstanceOverrideList();
                UnityEngine.Debug.Log("Tree reference changed");
            }

            if (GUILayout.Button("Open editor"))
            {
                if (treeExecutor.tree == null)
                {
                    treeExecutor.tree = CreateInstance<BehaviorTree>();
                    treeExecutor.tree.SaveToNew("Behavior Tree");
                }
                BehaviorTreeEditorView.OpenBehaviorTreeWindow().Load(treeExecutor.tree);
            }
            //for (int i = 0; i < treeExecutor.tree.blackboard.GameObjects.Count; i++)
            //{
            //    TypedVariable<GameObject> item = treeExecutor.tree.blackboard.GameObjects[i];
            //    treeExecutor.tree.blackboard.GameObjects[i].value = (GameObject)EditorGUILayout.ObjectField(item.name, item.value, typeof(GameObject), true);
            //}
        }
    }
}