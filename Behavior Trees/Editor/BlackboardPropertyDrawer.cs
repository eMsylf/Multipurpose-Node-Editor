using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.AI.BehaviorTree
{
    [CustomPropertyDrawer(typeof(Blackboard))]
    public class BlackboardPropertyDrawer : PropertyDrawer
    {
        bool fold = false;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty list = property.FindPropertyRelative("masterList");
            fold = EditorGUI.PropertyField(position, list);
            if (fold)
            {
                for (int i = 0; i < list.arraySize; i++)
                {
                    SerializedProperty listItem = list.GetArrayElementAtIndex(i);
                    //listItem.value
                }
            }
            EditorGUI.EndProperty();
        }
    }
}