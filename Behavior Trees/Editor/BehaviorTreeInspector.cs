using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes.AI.BehaviorTree;

[CustomEditor(typeof(BehaviorTree))]
public class BehaviorTreeInspector : Editor
{
    bool editNames;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BehaviorTree tree = (target as BehaviorTree);

        GUILayout.Label("Variables");
        editNames = EditorGUILayout.ToggleLeft("Edit names", editNames);
        DrawPropertiesForEach(tree.blackboard.bools);
        DrawPropertiesForEach(tree.blackboard.ints);
        DrawPropertiesForEach(tree.blackboard.floats);
        DrawPropertiesForEach(tree.blackboard.strings);
        DrawPropertiesForEach(tree.blackboard.Colors);
        DrawPropertiesForEach(tree.blackboard.Vector2s);
        DrawPropertiesForEach(tree.blackboard.Vector3s);
        DrawPropertiesForEach(tree.blackboard.Vector4s);
        DrawPropertiesForEach(tree.blackboard.Vector2Ints);
        DrawPropertiesForEach(tree.blackboard.Vector3Ints);
        DrawPropertiesForEach(tree.blackboard.GameObjects);

        if (GUILayout.Button("Add variable"))
        {
            GenericMenu contextMenu = new GenericMenu();
            //foreach (var propertyType in UnityEditorUtilities.propertyTypeLookup)
            //{
            //    Type type = propertyType.Value;
            //    contextMenu.AddItem(new GUIContent(propertyType.Key.ToString()), false, () => tree.blackboard.variables.Add(Variable.Create(propertyType.Value)));
            //}
            
            contextMenu.AddItem(new GUIContent("Boolean"), false, () => tree.AddVariable(typeof(bool)));
            contextMenu.AddItem(new GUIContent("Integer"), false, () => tree.AddVariable(typeof(int)));
            contextMenu.AddItem(new GUIContent("Float"), false, () => tree.AddVariable(typeof(float)));
            contextMenu.AddItem(new GUIContent("String"), false, () => tree.AddVariable(typeof(string)));
            contextMenu.AddItem(new GUIContent("Color"), false, () => tree.AddVariable(typeof(Color)));
            contextMenu.AddItem(new GUIContent("Vector2"), false, () => tree.AddVariable(typeof(Vector2)));
            contextMenu.AddItem(new GUIContent("Vector3"), false, () => tree.AddVariable(typeof(Vector3)));
            contextMenu.AddItem(new GUIContent("Vector4"), false, () => tree.AddVariable(typeof(Vector4)));
            contextMenu.AddItem(new GUIContent("Vector2Int"), false, () => tree.AddVariable(typeof(Vector2Int)));
            contextMenu.AddItem(new GUIContent("Vector3Int"), false, () => tree.AddVariable(typeof(Vector3Int)));
            contextMenu.AddItem(new GUIContent("GameObject"), false, () => tree.AddVariable(typeof(GameObject)));
            contextMenu.ShowAsContext();
        }
    }

    public void DrawPropertiesForEach<T>(List<TypedVariable<T>> typedVariables)
    {
        for (int i = 0; i < typedVariables.Count; i++)
        {
            AutoDrawProperty(typedVariables[i]);
        }
    }

    public void AutoDrawProperty<T>(TypedVariable<T> variable)
    {
        GUILayout.BeginHorizontal();
        if (editNames)
            variable.name = EditorGUILayout.TextField(variable.name);
        Draw(variable);
        GUILayout.EndHorizontal();
    }

    private void Draw<T>(TypedVariable<T> variable)
    {
        string label = "";
        if (!editNames)
        {
            label = variable.name;
            if (string.IsNullOrEmpty(label)) label = " ";
        }

        GUIContent labelContent = new GUIContent(label, "Value");
        switch (variable)
        {
            case TypedVariable<int> intVar:
                intVar.value = EditorGUILayout.IntField(labelContent, intVar.value);
                break;
            case TypedVariable<bool> boolVar:
                boolVar.value = EditorGUILayout.Toggle(labelContent, boolVar.value);
                break;
            case TypedVariable<float> floatVar:
                floatVar.value = EditorGUILayout.FloatField(labelContent, floatVar.value);
                break;
            case TypedVariable<string> stringVar:
                stringVar.value = EditorGUILayout.TextField(labelContent, stringVar.value);
                break;
            case TypedVariable<Color> colorVar:
                colorVar.value = EditorGUILayout.ColorField(labelContent, colorVar.value);
                break;
            case TypedVariable<Vector2> vector2Var:
                vector2Var.value = EditorGUILayout.Vector2Field(labelContent, vector2Var.value);
                break;
            case TypedVariable<Vector3> vector3Var:
                vector3Var.value = EditorGUILayout.Vector3Field(labelContent, vector3Var.value);
                break;
            case TypedVariable<Vector4> vector4Var:
                vector4Var.value = EditorGUILayout.Vector4Field(labelContent, vector4Var.value);
                break;
            case TypedVariable<Vector2Int> vectorInt2Var:
                vectorInt2Var.value = EditorGUILayout.Vector2IntField(labelContent, vectorInt2Var.value);
                break;
            case TypedVariable<Vector3Int> vectorInt3Var:
                vectorInt3Var.value = EditorGUILayout.Vector3IntField(labelContent, vectorInt3Var.value);
                break;
            case TypedVariable<GameObject> gameObjectVar:
                gameObjectVar.value = (GameObject)EditorGUILayout.ObjectField(labelContent, gameObjectVar.value, typeof(GameObject), true);
                break;
        }
    }
}
