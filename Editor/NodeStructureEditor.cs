using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.Callbacks;
using BobJeltes.AI.BehaviorTrees;
using BobJeltes.NodeEditor;

[CustomEditor(typeof(NodeStructure))]
public class NodeStructureEditor : Editor
{
    public static Dictionary<Type, string> typedEditorNames = new Dictionary<Type, string>
    {
        {typeof(NodeStructure), "Node editor" },
        {typeof(BehaviorTree), "Behavior tree" }
    };

    [OnOpenAsset()]
    public static bool Open(int instanceID, int line)
    {
        UnityEngine.Object asset = EditorUtility.InstanceIDToObject(instanceID);
        BehaviorTree behaviorTree = asset as BehaviorTree;
        if (behaviorTree)
        {
            var editor = BehaviorTreeEditorView.OpenBehaviorTreeWindow();
            editor.Load(behaviorTree);
            string name = EditorUtility.InstanceIDToObject(instanceID).name;
            return true;
        }

        NodeStructure nodeStructure = asset as NodeStructure;
        if (nodeStructure)
        {
            var editor = NodeBasedEditorView.OpenWindow();
            editor.Load(asset as NodeStructure);
            string name = EditorUtility.InstanceIDToObject(instanceID).name;
            return true;
        }
        return false;
    }
}
