using BobJeltes.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public class BehaviorTreeEditor : NodeBasedEditor
    {
        [MenuItem("Tools/Bob Jeltes/Behavior Tree editor")]
        [MenuItem("Window/Bob Jeltes/Behavior Tree editor")]
        public static BehaviorTreeEditor OpenBehaviorTreeWindow()
        {
            BehaviorTreeEditor openedWindow = GetWindow<BehaviorTreeEditor>("Behavior Tree editor");
            openedWindow.saveChangesMessage = "This behavior tree has not been saved. Would you like to save?";
            return openedWindow;
        }
    }
}