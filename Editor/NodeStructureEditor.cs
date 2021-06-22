using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditor;
using System;
using UnityEditor.Callbacks;
using BobJeltes.AI.BehaviorTree;

[CustomEditor(typeof(NodeStructure))]
public class NodeStructureEditor : Editor
{
    [OnOpenAsset()]
    public static bool Open(int instanceID, int line)
    {
        UnityEngine.Object asset = EditorUtility.InstanceIDToObject(instanceID);
        if (asset.GetType() == typeof(NodeStructure))
        {
            NodeBasedEditor editor = NodeBasedEditor.OpenWindow();
            editor.Load(asset as NodeStructure);
            string name = EditorUtility.InstanceIDToObject(instanceID).name;
            Debug.Log("Open " + name);
        }
        //Debug.Log("Line: " + line);
        return false;
    }
}
