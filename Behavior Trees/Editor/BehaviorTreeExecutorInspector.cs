using BobJeltes.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    [CustomEditor(typeof(BehaviorTreeExecutor))]
    public class BehaviorTreeExecutorInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            BehaviorTreeExecutor treeExecutor = (BehaviorTreeExecutor)target;

            // TODO: Upon changing the behavior tree reference, wipe the blackboard instance copy
            //base.OnInspectorGUI();
            DrawPropertiesExcluding(serializedObject, "script", "tree");
            serializedObject.ApplyModifiedProperties();
            SerializedProperty tree = serializedObject.FindProperty("tree");
            EditorGUI.BeginChangeCheck();
            treeExecutor.tree = (BehaviorTree)EditorGUILayout.ObjectField("Tree", treeExecutor.tree, typeof(BehaviorTree), false);
            if (EditorGUI.EndChangeCheck())
            {
                treeExecutor.blackboardInstanceCopy = new Blackboard();
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

            if (GUILayout.Button("Create instance variable override"))
            {
                GenericMenu instanceOverrideMenu = new GenericMenu();
                treeExecutor.tree.blackboard.GameObjects.ForEach(x =>
                {
                    TypedVariable<GameObject> typedVariableInstance = TypedVariable<GameObject>.CopyData(x);
                    if (treeExecutor.blackboardInstanceCopy.GameObjects.Exists(y => y.ID == x.ID))
                    {
                        instanceOverrideMenu.AddDisabledItem(new GUIContent(typedVariableInstance.name), false);
                    }
                    else
                    {
                        instanceOverrideMenu.AddItem(new GUIContent(typedVariableInstance.name), false, () => treeExecutor.blackboardInstanceCopy.GameObjects.Add(x));
                    }
                });
                instanceOverrideMenu.ShowAsContext();
            }
        }
    }
}