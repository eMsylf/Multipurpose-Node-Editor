using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BobJeltes.AI.BehaviorTrees
{
    //[CustomPropertyDrawer(typeof(Blackboard))]
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
                    //Variable objectListItem = (Variable)listItem.serializedObject.targetObject;
                    //switch (objectListItem)
                    //{
                    //    case objectListItem as TypedVariable<GameObject>:

                    //        break;
                    //    default:
                    //        break;
                    //}
                }
            }
            EditorGUI.EndProperty();
        }

        public class VariableTableSettings
        {
            public VariableTableSettings(params Column[] columns)
            {

            }

            public class Column
            {
                public enum WidthCalculation
                {
                    Fixed,
                    Flexible,
                    Percentage
                }
                WidthCalculation widthType;
                string title = "";
                int minWidth = 0;
                int maxWidth = 0;
                float percentageWidth = 0;

                public Column(string title, int width)
                {
                    widthType = WidthCalculation.Fixed;
                    this.title = title;
                    minWidth = width;
                    maxWidth = width;
                }

                public Column(string title, int minWidth, int maxWidth)
                {
                    this.widthType = WidthCalculation.Flexible;
                    this.title = title;
                    this.minWidth = minWidth;
                    this.maxWidth = maxWidth;
                }

                public Column(string title, float percentage)
                {
                    widthType = WidthCalculation.Percentage;
                    this.title = title;
                    this.percentageWidth = percentage;
                }
            }
        }
    }
}