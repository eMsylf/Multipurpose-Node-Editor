using BobJeltes.AI.BehaviorTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    public class BehaviorTreeView : NodeBasedEditorView
    {
        [MenuItem("Tools/Bob Jeltes/Behavior Tree editor")]
        [MenuItem("Window/Bob Jeltes/Behavior Tree editor")]
        public static BehaviorTreeView OpenBehaviorTreeWindow()
        {
            BehaviorTreeView openedWindow = CreateWindow<BehaviorTreeView>("Behavior Tree editor");
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

        internal override void PopulateView(NodeStructure nodeStructure)
        {
            BehaviorTree behaviorTree = nodeStructure as BehaviorTree;
            nodeViews = new List<NodeView>();
            nodeViews.Add(CreateRootNode());
            foreach (var node in behaviorTree.nodes)
            {
                Debug.Log(node.GetType().Name);
                behaviorTree.nodes.ForEach(n => CreateNodeView(n));
            }
        }

        internal override void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            var composites = TypeCache.GetTypesDerivedFrom<Composite>();
            foreach (var nodeType in composites)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Composite/" + nodeType.Name), false, () => CreateNodeView(mousePosition).title = nodeType.Name);
            }
            var decorators = TypeCache.GetTypesDerivedFrom<Decorator>();
            foreach (var nodeType in decorators)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Decorator/" + nodeType.Name), false, () => CreateNodeView(mousePosition).title = nodeType.Name);
            }
            var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var nodeType in actionNodes)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Actions/" + nodeType.Name), false, () => CreateNodeView(mousePosition).title = nodeType.Name);
            }
            genericMenu.ShowAsContext();
        }

        protected override void OnClickBackgroundWithConnection()
        {
            ProcessContextMenu(Event.current.mousePosition);
        }

        public override void SaveChanges()
        {
            base.SaveChanges();
            BehaviorTree behaviorTree = reference as BehaviorTree;
            foreach (var nodeView in nodeViews)
            {

            }
        }

        internal NodeView CreateRootNode()
        {
            preset1.Load(out GUIStyle rootNodeStyle, out GUIStyle rootNodeStyleSelected);
            NodeView root = new NodeView(new Rect(position.size * .5f, new Vector2(200, 50)), orientation, rootNodeStyle, rootNodeStyleSelected, GUIStyle.none, outPointStyle, null, OnClickConnectionPoint, null, null, null);
            CreateNodeView(root);
            Debug.Log("Create root node");
            return root;
        }

        /// <summary>
        /// Create a behavior tree node view from a behavior tree node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView CreateNodeView(Node node)
        {
            NodeView nodeView = new NodeView(node);
            nodeView.title = node.GetType().Name;
            return nodeView;
        }

        //public Node CreateNode(NodeView nodeView)
        //{
        //    if (BehaviorTree.GetNodeType(nodeView.title, out Type nodeType))
        //    {
        //        Node node = new Node() as nodeType;

        //    }
        //}
    }
}