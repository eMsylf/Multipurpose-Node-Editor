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

            treeExecutor.tree = (BehaviorTree)EditorGUILayout.ObjectField("Tree", treeExecutor.tree, typeof(BehaviorTree), false);
            
            SerializedProperty blackboard = serializedObject.FindProperty("variableOverrides");
            EditorGUILayout.PropertyField(blackboard);
            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
            {
                treeExecutor.variableOverrides = new Blackboard();
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

            if (GUILayout.Button("Create variable override"))
            {
                GenericMenu instanceOverrideMenu = new GenericMenu();
                CreateListMenuItems(treeExecutor.tree.blackboard.bools, treeExecutor.variableOverrides.bools, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.ints, treeExecutor.variableOverrides.ints, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.floats, treeExecutor.variableOverrides.floats, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.strings, treeExecutor.variableOverrides.strings, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Colors, treeExecutor.variableOverrides.Colors, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Vector2s, treeExecutor.variableOverrides.Vector2s, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Vector3s, treeExecutor.variableOverrides.Vector3s, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Vector4s, treeExecutor.variableOverrides.Vector4s, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Vector2Ints, treeExecutor.variableOverrides.Vector2Ints, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.Vector3Ints, treeExecutor.variableOverrides.Vector3Ints, instanceOverrideMenu);
                CreateListMenuItems(treeExecutor.tree.blackboard.GameObjects, treeExecutor.variableOverrides.GameObjects, instanceOverrideMenu);
                instanceOverrideMenu.ShowAsContext();
            }
        }

        private static void CreateListMenuItems<T>(List<TypedVariable<T>> originalList, List<TypedVariable<T>> targetList, GenericMenu instanceOverrideMenu)
        {
            foreach (var variable in originalList)
            {
                TypedVariable<T> typedVariableInstance = TypedVariable<T>.CopyData(variable);
                GUIContent text = new GUIContent(typedVariableInstance.name + " ( " + typedVariableInstance.value + " )");
                if (targetList.Exists(y => y.ID == variable.ID))
                {
                    instanceOverrideMenu.AddDisabledItem(text, false);
                }
                else
                {
                    instanceOverrideMenu.AddItem(text, false, () => targetList.Add(new TypedVariable<T>(variable.id) { name = variable.name }));
                }
            }
        }
    }
}