using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes.AI.BehaviorTree;

[CustomPropertyDrawer(typeof(Variable))]
public class VariablePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label.text = property.FindPropertyRelative("name").stringValue;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Generic:
                break;
            case SerializedPropertyType.Integer:
                property.intValue = EditorGUI.IntField(position, label, property.intValue);
                break;
            case SerializedPropertyType.Boolean:
                property.boolValue = EditorGUI.Toggle(position, label, property.boolValue);
                break;
            case SerializedPropertyType.Float:
                property.floatValue = EditorGUI.FloatField(position, label, property.floatValue);
                break;
            case SerializedPropertyType.String:
                property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
                break;
            case SerializedPropertyType.Color:
                property.colorValue = EditorGUI.ColorField(position, label, property.colorValue);
                break;
            case SerializedPropertyType.ObjectReference:
                
                break;
            case SerializedPropertyType.LayerMask:
                
                break;
            case SerializedPropertyType.Enum:
                break;
            case SerializedPropertyType.Vector2:
                property.vector2Value = EditorGUI.Vector2Field(position, label, property.vector2Value);
                break;
            case SerializedPropertyType.Vector3:
                property.vector3Value = EditorGUI.Vector3Field(position, label, property.vector3Value);
                break;
            case SerializedPropertyType.Vector4:
                property.vector4Value = EditorGUI.Vector4Field(position, label, property.vector4Value);
                break;
            case SerializedPropertyType.Rect:
                break;
            case SerializedPropertyType.ArraySize:
                break;
            case SerializedPropertyType.Character:
                break;
            case SerializedPropertyType.AnimationCurve:
                break;
            case SerializedPropertyType.Bounds:
                break;
            case SerializedPropertyType.Gradient:
                break;
            case SerializedPropertyType.Quaternion:
                break;
            case SerializedPropertyType.ExposedReference:
                break;
            case SerializedPropertyType.FixedBufferSize:
                break;
            case SerializedPropertyType.Vector2Int:
                break;
            case SerializedPropertyType.Vector3Int:
                break;
            case SerializedPropertyType.RectInt:
                break;
            case SerializedPropertyType.BoundsInt:
                break;
            case SerializedPropertyType.ManagedReference:
                break;
            default:
                break;
        }
    }
}
