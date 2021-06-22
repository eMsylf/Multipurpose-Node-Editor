using BobJeltes.AI.BehaviorTree;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    public class BehaviorTreeEditor : NodeBasedEditor
    {
        [MenuItem("Tools/Bob Jeltes/Behavior Tree editor")]
        [MenuItem("Window/Bob Jeltes/Behavior Tree editor")]
        public static BehaviorTreeEditor OpenBehaviorTreeEditor()
        {
            BehaviorTreeEditor openedWindow = CreateWindow<BehaviorTreeEditor>("Behavior Tree editor");
            openedWindow.saveChangesMessage = "This behavior tree has not been saved. Would you like to save?";
            return openedWindow;
        }

        public class NodeStyle
        {
            public string normalBackground;
            public string selectedBackground;
            public RectOffset border;

            public NodeStyle(string normalBackground, string selectedBackground, RectOffset border)
            {
                this.normalBackground = normalBackground;
                this.selectedBackground = selectedBackground;
                this.border = border;
            }

            public void Load(out GUIStyle normalStyle, out GUIStyle selectedStyle)
            {
                normalStyle = new GUIStyle();
                normalStyle.normal.background = EditorGUIUtility.Load(normalBackground) as Texture2D;
                normalStyle.border = border;

                selectedStyle = new GUIStyle();
                selectedStyle.normal.background = EditorGUIUtility.Load(selectedBackground) as Texture2D;
                selectedStyle.border = border;
            }
        }

        public NodeStyle preset1 = new NodeStyle(
            "builtin skins/darkskin/images/node1.png",
            "builtin skins/darkskin/images/node1 on.png",
            new RectOffset(12, 12, 12, 12));

        public NodeStyle preset2 = new NodeStyle(
            "builtin skins/darkskin/images/node2.png",
            "builtin skins/darkskin/images/node2 on.png",
            new RectOffset(12, 12, 12, 12));

        public NodeStyle preset3 = new NodeStyle(
            "builtin skins/darkskin/images/node3.png",
            "builtin skins/darkskin/images/node3 on.png",
            new RectOffset(12, 12, 12, 12));

        public NodeStyle preset4 = new NodeStyle(
            "builtin skins/darkskin/images/node4.png",
            "builtin skins/darkskin/images/node4 on.png",
            new RectOffset(12, 12, 12, 12));

        public NodeStyle preset5 = new NodeStyle(
            "builtin skins/darkskin/images/node5.png",
            "builtin skins/darkskin/images/node5 on.png",
            new RectOffset(12, 12, 12, 12));

        public NodeStyle redPreset = new NodeStyle(
            "builtin skins/darkskin/images/node6.png", 
            "builtin skins/darkskin/images/node6 on.png", 
            new RectOffset(12, 12, 12, 12));

        internal override void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            var composites = TypeCache.GetTypesDerivedFrom<Composite>();
            foreach (var node in composites)
            {
                Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Composite/" + node.Name), false, () => OnClickAddNode(mousePosition, true));
            }
            var decorators = TypeCache.GetTypesDerivedFrom<Decorator>();
            foreach (var node in decorators)
            {
                Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Decorator/" + node.Name), false, () => OnClickAddNode(mousePosition, true));
            }
            var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var node in actionNodes)
            {
                Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Actions/" + node.Name), false, () => OnClickAddNode(mousePosition, true));
            }
            genericMenu.ShowAsContext();
        }
    }
}