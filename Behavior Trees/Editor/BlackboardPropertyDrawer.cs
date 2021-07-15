using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes.AI.BehaviorTree;

[CustomPropertyDrawer(typeof(Blackboard))]
public class BlackboardPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        //EditorGUI.DropdownButton()
        base.OnGUI(position, property, label);
    }
}
