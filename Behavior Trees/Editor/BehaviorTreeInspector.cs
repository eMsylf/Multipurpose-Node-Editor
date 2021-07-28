using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BobJeltes.AI.BehaviorTrees;

[CustomEditor(typeof(BehaviorTree))]
public class BehaviorTreeInspector : Editor
{
    bool showVariables;
    bool editNames;
    GUIStyle boldHeaderWithUnderline;
    int tableRowVerticalPadding = 4;
    GUIStyle tableRowBrightStyle;
    GUIStyle tableRowDarkStyle;

    private void OnEnable()
    {
        boldHeaderWithUnderline = new GUIStyle() { fontStyle = FontStyle.Bold };
        boldHeaderWithUnderline.normal.textColor = Color.white;
        boldHeaderWithUnderline.border.left = -5;
        boldHeaderWithUnderline.padding.left = 2;
        boldHeaderWithUnderline.padding.right = 2;
        boldHeaderWithUnderline.padding.bottom = 2;
        boldHeaderWithUnderline.border.bottom = -2;
        boldHeaderWithUnderline.margin.top = 5;
        boldHeaderWithUnderline.margin.bottom = 2;

        Texture2D typeHeaderTexture = new Texture2D(1, 1);
        typeHeaderTexture.SetPixel(0, 0, new Color(0, 0, 0, .25f));
        typeHeaderTexture.Apply();
        boldHeaderWithUnderline.normal.background = typeHeaderTexture;

        tableRowBrightStyle = new GUIStyle();
        Texture2D brightTexture = new Texture2D(1, 1);
        brightTexture.SetPixel(0, 0, new Color(0, 0, 0, .1f));
        brightTexture.Apply();
        tableRowBrightStyle.normal.background = brightTexture;
        tableRowBrightStyle.padding.top = tableRowVerticalPadding;
        tableRowBrightStyle.padding.bottom = tableRowVerticalPadding;

        tableRowDarkStyle = new GUIStyle();
        Texture2D darkTexture = new Texture2D(1, 1);
        darkTexture.SetPixel(0, 0, new Color(0, 0, 0, .2f));
        darkTexture.Apply();
        tableRowDarkStyle.normal.background = darkTexture;
        tableRowDarkStyle.padding.top = tableRowVerticalPadding;
        tableRowDarkStyle.padding.bottom = tableRowVerticalPadding;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //currentMenu = GUILayout.Toolbar(currentMenu, new string[] { "Nodes", "Variables"});
        DrawVariables();
    }

    public void DrawVariables()
    {
        BehaviorTree tree = (target as BehaviorTree);
        if (editNames)
        {
            if (GUILayout.Button("Save names"))
            {
                editNames = false;
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            if (GUILayout.Button("Edit names"))
            {
                editNames = true;
            }
        }
        //DrawPropertiesForEach(tree.blackboard.masterList);
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

    public void DrawPropertiesForEach(List<Variable> variables)
    {
        for (int i = 0; i < variables.Count; i++)
        {
            AutoDrawVariable(variables[i]);
        }
    }

    public void AutoDrawVariable(Variable variable)
    {
        GUILayout.BeginHorizontal();
        if (editNames)
            variable.name = EditorGUILayout.TextField(variable.name);
        Draw(variable);
        GUILayout.EndHorizontal();
    }
    public void DrawPropertiesForEach<T>(List<TypedVariable<T>> typedVariables)
    {
        if (typedVariables.Count == 0) return;
        EditorGUILayout.BeginHorizontal(boldHeaderWithUnderline);
        string label2 = typeof(T) == typeof(GameObject) ? "Scene" : "";
        EditorGUILayout.LabelField(typeof(T).Name + "s", label2);
        EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(50));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+"))
        {
            (target as BehaviorTree).AddVariable(typeof(T));
        }
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < typedVariables.Count; i++)
        {
            GUIStyle currentStyle = i % 2 == 0 ? tableRowBrightStyle : tableRowDarkStyle;
            GUILayout.BeginHorizontal(currentStyle);
            if (GUILayout.Button("-", GUILayout.Width(17f)))
            {
                Undo.RecordObject(target, "Remove variable");
                (target as BehaviorTree).RemoveVariable(typedVariables[i]);
                i--;
            }
            else
            {
                Draw(typedVariables[i]);
            }

            GUILayout.EndHorizontal();
        }
    }

    private void Draw(Variable variable)
    {
        string label = "";
        if (variable == null)
        {
            Debug.LogError("Variable is null");
            return;
        }
        if (editNames)
        {
            // Show edit field
            Color originalGUIColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0, 1, 1);
            variable.name = EditorGUILayout.TextField(variable.name);
            GUI.backgroundColor = originalGUIColor;
        }
        else
        {
            // Show name
            label = variable.name;
            if (string.IsNullOrEmpty(label)) label = " ";
        }
        switch (variable)
        {
            case TypedVariable<int> intVar:
                intVar.value = EditorGUILayout.IntField(label, intVar.value);
                break;
            case TypedVariable<bool> boolVar:
                boolVar.value = EditorGUILayout.Toggle(label, boolVar.value);
                break;
            case TypedVariable<float> floatVar:
                floatVar.value = EditorGUILayout.FloatField(label, floatVar.value);
                break;
            case TypedVariable<string> stringVar:
                stringVar.value = EditorGUILayout.TextField(label, stringVar.value);
                break;
            case TypedVariable<Color> colorVar:
                colorVar.value = EditorGUILayout.ColorField(label, colorVar.value);
                break;
            case TypedVariable<Vector2> vector2Var:
                vector2Var.value = EditorGUILayout.Vector2Field(label, vector2Var.value);
                break;
            case TypedVariable<Vector3> vector3Var:
                vector3Var.value = EditorGUILayout.Vector3Field(label, vector3Var.value);
                break;
            case TypedVariable<Vector4> vector4Var:
                vector4Var.value = EditorGUILayout.Vector4Field(label, vector4Var.value);
                break;
            case TypedVariable<Vector2Int> vectorInt2Var:
                vectorInt2Var.value = EditorGUILayout.Vector2IntField(label, vectorInt2Var.value);
                break;
            case TypedVariable<Vector3Int> vectorInt3Var:
                vectorInt3Var.value = EditorGUILayout.Vector3IntField(label, vectorInt3Var.value);
                break;
            case TypedVariable<GameObject> gameObjectVar:
                EditorGUILayout.PrefixLabel(label);
                gameObjectVar.isSceneReference = EditorGUILayout.Toggle(gameObjectVar.isSceneReference, GUILayout.MaxWidth(30f));
                if (gameObjectVar.isSceneReference)
                {
                    EditorGUILayout.LabelField("Assign in Behavior Tree Executor component");
                }
                else 
                    gameObjectVar.value = (GameObject)EditorGUILayout.ObjectField(gameObjectVar.value, typeof(GameObject), false);

                break;
        }
    }
}
