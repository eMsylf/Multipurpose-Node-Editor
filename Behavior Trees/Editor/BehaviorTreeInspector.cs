using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes.AI.BehaviorTree;
using System;

[CustomEditor(typeof(BehaviorTree))]
public class BehaviorTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BehaviorTree tree = (target as BehaviorTree);

        if (GUILayout.Button("Add variable"))
        {
            GenericMenu contextMenu = new GenericMenu();
            foreach (var typeName in propertyType)
            {
                Type type = typeName.Value;
                contextMenu.AddItem(new GUIContent(type.Name), false, () => tree.blackboard.variables.Add(Variable.Create(type)));
            }
            contextMenu.ShowAsContext();
        }
    }

    public Dictionary<SerializedPropertyType, Type> propertyType = new Dictionary<SerializedPropertyType, Type>
    {
        { SerializedPropertyType.Integer, typeof(int) },
        { SerializedPropertyType.Boolean, typeof(bool) },
        { SerializedPropertyType.Float, typeof(float) },
        { SerializedPropertyType.String, typeof(string) },
        { SerializedPropertyType.Vector2, typeof(Vector2) },
        { SerializedPropertyType.Vector3, typeof(Vector3) },
        //{ SerializedPropertyType.Vector4, typeof(Vector4) },
        //{ SerializedPropertyType.Boolean, typeof(bool) },
        //{ SerializedPropertyType.Boolean, typeof(bool) },
        //{ SerializedPropertyType.Boolean, typeof(bool) },
        //{ SerializedPropertyType.Boolean, typeof(bool) },
        //{ SerializedPropertyType.Boolean, typeof(bool) }
    };

    public void GetSerializedPropertyType()
    {
        UnityEngine.Debug.Log("Ayy lmao");
    }
}
