using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditor;

public class BehaviorTreeEditor : NodeBasedEditor
{
    [MenuItem("Window/Bob Jeltes/Behavior Tree Editor")]
    public static new BehaviorTreeEditor OpenWindow()
    {
        BehaviorTreeEditor openedWindow = GetWindow<BehaviorTreeEditor>("Behavior Tree Editor");
        openedWindow.saveChangesMessage = "This behavior tree has not been saved. Would you like to save?";
        return openedWindow;
    }

    internal override Node OnClickAddNode(Vector2 mousePosition, bool centered)
    {
        Debug.Log("Create behavior tree node");
        return base.OnClickAddNode(mousePosition, centered);
    }

    public override void SaveChanges()
    {
        // TODO: Save in Assets/Resources/Behavior Trees
        base.SaveChanges();
    }
}
