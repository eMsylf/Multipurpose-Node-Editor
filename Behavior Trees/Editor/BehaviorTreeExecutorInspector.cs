using BobJeltes.AI.BehaviorTrees;
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

            EditorGUI.BeginChangeCheck();
            treeExecutor.tree = (BehaviorTree)EditorGUILayout.ObjectField("Tree", treeExecutor.tree, typeof(BehaviorTree), false);
            if (EditorGUI.EndChangeCheck())
            {
                treeExecutor.RebuildInstanceOverrideList();
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

            SerializedProperty blackboard = serializedObject.FindProperty("variableOverrides");
            DrawBlackboard(treeExecutor, blackboard);
        }

        private static void DrawBlackboard(BehaviorTreeExecutor treeExecutor, SerializedProperty blackboard)
        {
            if (treeExecutor.tree == null) return;
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Blackboard");
            EditorGUILayout.PropertyField(blackboard);
            //if (GUILayout.Button("Create variable override"))
            //{
            //    treeExecutor.RebuildInstanceOverrideList();
            //    GenericMenu instanceOverrideMenu = new GenericMenu();
            //    CreateListMenuItems(treeExecutor.tree.blackboard.bools, treeExecutor.variableOverrides.bools, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.ints, treeExecutor.variableOverrides.ints, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.floats, treeExecutor.variableOverrides.floats, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.strings, treeExecutor.variableOverrides.strings, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Colors, treeExecutor.variableOverrides.Colors, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Vector2s, treeExecutor.variableOverrides.Vector2s, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Vector3s, treeExecutor.variableOverrides.Vector3s, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Vector4s, treeExecutor.variableOverrides.Vector4s, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Vector2Ints, treeExecutor.variableOverrides.Vector2Ints, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.Vector3Ints, treeExecutor.variableOverrides.Vector3Ints, instanceOverrideMenu);
            //    CreateListMenuItems(treeExecutor.tree.blackboard.GameObjects, treeExecutor.variableOverrides.GameObjects, instanceOverrideMenu);
            //    if (instanceOverrideMenu.GetItemCount() == 0)
            //    {
            //        Debug.Log("You have not added any variables to the behavior tree's blackboard that can be overriden. Please do that first.", treeExecutor.tree);
            //        instanceOverrideMenu.AddDisabledItem(new GUIContent("No variables to override"));
            //    }
            //    instanceOverrideMenu.ShowAsContext();
            //}
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