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
        public static BehaviorTreeEditor OpenBehaviorTreeWindow()
        {
            BehaviorTreeEditor openedWindow = CreateWindow<BehaviorTreeEditor>("Behavior Tree editor");
            openedWindow.saveChangesMessage = "This behavior tree has not been saved. Would you like to save?";
            return openedWindow;
        }

        public class NodeStyle
        {
            public string normalBackground;
            public string selectedBackground;
            public int border;

            public NodeStyle(string normalBackground, string selectedBackground, int border)
            {
                this.normalBackground = normalBackground;
                this.selectedBackground = selectedBackground;
                this.border = border;
            }

            public void Load(out GUIStyle normalStyle, out GUIStyle selectedStyle)
            {
                normalStyle = new GUIStyle();
                normalStyle.normal.background = EditorGUIUtility.Load(normalBackground) as Texture2D;
                normalStyle.border = new RectOffset(border, border, border, border);

                selectedStyle = new GUIStyle();
                selectedStyle.normal.background = EditorGUIUtility.Load(selectedBackground) as Texture2D;
                selectedStyle.border = new RectOffset(border, border, border, border);
            }
        }

        public NodeStyle preset1 = new NodeStyle(
            "builtin skins/darkskin/images/node1.png",
            "builtin skins/darkskin/images/node1 on.png",
            12);

        public NodeStyle preset2 = new NodeStyle(
            "builtin skins/darkskin/images/node2.png",
            "builtin skins/darkskin/images/node2 on.png",
            12);

        public NodeStyle preset3 = new NodeStyle(
            "builtin skins/darkskin/images/node3.png",
            "builtin skins/darkskin/images/node3 on.png",
            12);

        public NodeStyle preset4 = new NodeStyle(
            "builtin skins/darkskin/images/node4.png",
            "builtin skins/darkskin/images/node4 on.png",
            12);

        public NodeStyle preset5 = new NodeStyle(
            "builtin skins/darkskin/images/node5.png",
            "builtin skins/darkskin/images/node5 on.png",
            12);

        public NodeStyle redPreset = new NodeStyle(
            "builtin skins/darkskin/images/node6.png", 
            "builtin skins/darkskin/images/node6 on.png", 
            12);

        internal override void InitNodes()
        {
            base.InitNodes();
            if (nodes == null || nodes.Count == 0)
            {
                CreateRootNode();
            }
        }

        internal void CreateRootNode()
        {
            preset1.Load(out GUIStyle rootNodeStyle, out GUIStyle rootNodeStyleSelected);
            NodeView root = new NodeView(new Rect(position.size * .5f, new Vector2(200, 50)), orientation, rootNodeStyle, rootNodeStyleSelected, GUIStyle.none, outPointStyle, null, OnClickConnectionPoint, null, null, null);
            CreateNode(root);
            Debug.Log("Create root node");
        }

        internal override void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            var composites = TypeCache.GetTypesDerivedFrom<Composite>();
            foreach (var node in composites)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Composite/" + node.Name), false, () => CreateNode(mousePosition).title = node.Name);
            }
            var decorators = TypeCache.GetTypesDerivedFrom<Decorator>();
            foreach (var node in decorators)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Decorator/" + node.Name), false, () => CreateNode(mousePosition).title = node.Name);
            }
            var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var node in actionNodes)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Actions/" + node.Name), false, () => CreateNode(mousePosition).title = node.Name);
            }
            genericMenu.ShowAsContext();
        }

        protected override void OnClickBackgroundWithConnection()
        {
            ProcessContextMenu(Event.current.mousePosition);
        }
    }
}