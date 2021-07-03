using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Int2))]
public class int2Editor : PropertyDrawer
{
    static GUIContent[] labels = new GUIContent[]
    {
        new GUIContent("a"),
        new GUIContent("b")
    };

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.MultiIntField(position,
                                labels,
                                new int[] { 
                                    property.FindPropertyRelative("a").intValue, 
                                    property.FindPropertyRelative("b").intValue 
                                });
    }
}
