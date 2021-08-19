using BobJeltes.AI.BehaviorTrees;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(Variable))]
public class VariablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.Label(position, property.type);
        //property.
        //switch (property)
        //{
        //    case TypedVariable<GameObject> gameObjectVariable:

        //        break;
        //    default:
        //        break;
        //}
    }
}
