using BobJeltes.AI.BehaviorTree;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    public class BehaviorTreeEditorView : NodeBasedEditorView
    {
        BehaviorTree behaviorTree;
        public Vector2 GetDefaultRootPosition() => new Vector2(position.width * .5f, 100f);
        [MenuItem("Tools/Bob Jeltes/Behavior Tree editor")]
        [MenuItem("Window/Bob Jeltes/Behavior Tree editor")]
        public static BehaviorTreeEditorView OpenBehaviorTreeWindow()
        {
            BehaviorTreeEditorView openedWindow = CreateWindow<BehaviorTreeEditorView>("Behavior Tree editor");
            openedWindow.saveChangesMessage = "This behavior tree has not been saved. Would you like to save?";
            return openedWindow;
        }

        internal override void PopulateView()
        {
            if (file == null) behaviorTree = CreateInstance<BehaviorTree>();
            else behaviorTree = (file as BehaviorTree).Clone();

            behaviorTree = file as BehaviorTree;

            if (behaviorTree == null) behaviorTree = CreateInstance<BehaviorTree>();
            
            nodeViews = new List<NodeView>();

            if (behaviorTree.root == null) behaviorTree.root = (RootNode)behaviorTree.CreateNode(typeof(RootNode));
            CreateNodeView(behaviorTree.root, GetDefaultRootPosition());

            for (int i = 0; i < behaviorTree.nodes.Count; i++)
            {
                CreateNodeView(behaviorTree.nodes[i].Clone(), behaviorTree.nodes[i].positionOnView, false);
            }
        }

        internal override void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            var composites = TypeCache.GetTypesDerivedFrom<Composite>();
            foreach (var nodeType in composites)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Composite/" + nodeType.Name), false, () => CreateNodeView(CreateNode(nodeType), mousePosition));
            }
            var decorators = TypeCache.GetTypesDerivedFrom<Decorator>();
            foreach (var nodeType in decorators)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Decorator/" + nodeType.Name), false, () => CreateNodeView(CreateNode(nodeType), mousePosition));
            }
            var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var nodeType in actionNodes)
            {
                //Debug.Log(node.Name);
                genericMenu.AddItem(new GUIContent("Actions/" + nodeType.Name), false, () => CreateNodeView(CreateNode(nodeType), mousePosition));
            }
            genericMenu.ShowAsContext();
        }

        Node CreateNode(Type type)
        {
            return behaviorTree.CreateNode(type);
        }

        internal override void OnClickRemoveNode(NodeView nodeView)
        {
            behaviorTree.DeleteNode(nodeView.node);
            base.OnClickRemoveNode(nodeView);
        }

        protected override void OnClickBackgroundWithConnection()
        {
            ProcessContextMenu(Event.current.mousePosition);
        }

        public override void Save()
        {
            // Check for unsaved changes
            if (!hasUnsavedChanges) return;

            // Secondly, add all the children from connections
            foreach (var nodeView in nodeViews)
            {
                var multipleChildrenNode = nodeView.node as NodeInterfaces.IMultipleConnection;
                if (multipleChildrenNode != null)
                {
                    // Clear the child list to 0 to remove any dangling copies that are not assigned to the behavior tree
                    multipleChildrenNode.SetChildren(new List<Node>());
                    // Find all child node views connected through the out port
                    List<Connection> outgoingConnections = connections.FindAll(c => c.outNodeView == nodeView);
                    // Get all the child nodes from those connections
                    List<NodeView> childNodes = outgoingConnections.ConvertAll(c => c.inNodeView);
                    foreach (var childNode in childNodes)
                    {
                        Node childNodeInTree = behaviorTree.nodes.Find(n => n.guid == childNode.node.guid);
                        if (childNodeInTree != null)
                        {
                            multipleChildrenNode.AddChild(childNodeInTree);
                        }
                    }
                    continue;
                }
                var singleChildrenNode = nodeView.node as NodeInterfaces.ISingleConnection;
                if (singleChildrenNode != null)
                {
                    // Find all child node views connected through the out port
                    List<Connection> outgoingConnections = connections.FindAll(c => c.outNodeView == nodeView);
                    UnityEngine.Debug.Log(nodeView.node.name + " has " + outgoingConnections.Count + " outgoing connections");
                    // Get all the child nodes from those connections
                    if (outgoingConnections.Count == 0)
                    {
                        singleChildrenNode.SetChild(null);
                    }
                    else
                    {
                        List<NodeView> childNodes = outgoingConnections.ConvertAll(c => c.inNodeView);
                        singleChildrenNode.SetChild(behaviorTree.nodes.Find(n => n.guid == childNodes[0].node.guid));
                    }
                    continue;
                }
            }

            UnityEngine.Debug.Log("Nodes saved: " + behaviorTree.nodes.Count);
            BehaviorTree btFile = file as BehaviorTree;
            if (btFile == null)
            {
                behaviorTree.SaveTo(btFile, fileName);
                file = behaviorTree;
            }
            else
                behaviorTree.SaveTo(btFile, btFile.name);


            // Remove "(Clone)" from name
            if (behaviorTree.name.Contains("(Clone)"))
            {
                behaviorTree.name = behaviorTree.name.Substring(0, behaviorTree.name.Length - 7);
            }
            
            
            base.SaveChanges();
            // Load the behavior tree to load a clone of the saved tree, and prevent working on the file directly.
            Load(btFile);
        }

        public bool Load(BehaviorTree tree)
        {
            if (!UnsavedChangesCheck()) return false;
            if (tree == null)
            {
                NewFile();
                return true;
            }


            //if (tree.root == null)
            //{
            //    tree.root = (RootNode)tree.CreateNode(typeof(RootNode));
            //    UnityEngine.Debug.Log("Create new root node from load");
            //}

            // Make a copy of the reference structure
            behaviorTree = tree.Clone();

            nodeViews = new List<NodeView>();
            connections = new List<Connection>();
            UnityEngine.Debug.Log("Found a root node and " + behaviorTree.nodes.Count + " nodes");

            // Create a view for the root node
            CreateNodeView(behaviorTree.root, GetDefaultRootPosition());

            // Create node views for every other node
            foreach (var node in behaviorTree.nodes)
            {
                CreateNodeView(node);
            }

            // Connect the node views
            foreach (var nodeView in nodeViews)
            {
                var multipleConnectionsNode = nodeView.node as NodeInterfaces.IMultipleConnection;
                if (multipleConnectionsNode != null)
                {
                    // For every child of the node, 
                    foreach (var nodeChild in multipleConnectionsNode.GetChildren())
                    {
                        CreateConnection(nodeView, nodeViews.Find(x => x.node.guid == nodeChild.guid));
                    }
                    continue;
                }
                var singleConnectionNode = nodeView.node as NodeInterfaces.ISingleConnection;
                if (singleConnectionNode != null)
                {
                    Node nodeChild = singleConnectionNode.GetChild();
                    if (nodeChild != null)
                    {
                        UnityEngine.Debug.Log(nodeView.node + " has a child");
                        CreateConnection(nodeView, nodeViews.Find(x => x.node.guid == nodeChild.guid));
                    }
                    else
                    {
                        UnityEngine.Debug.Log(nodeView.node + " has no child");
                    }
                    continue;
                }
            }

            // Set the reference to the reference file
            file = tree;
            hasUnsavedChanges = false;
            ClearConnectionSelection();
            return true;
        }
    }

    public class NodeViewStyle
    {
        public string normalBackground;
        public string selectedBackground;
        public int border;
        public Vector2 size;

        public NodeViewStyle(string normalBackground, string selectedBackground, int border, Vector2 size)
        {
            this.normalBackground = normalBackground;
            this.selectedBackground = selectedBackground;
            this.border = border;
            this.size = size;
        }

        public void Load(out GUIStyle normalStyle, out GUIStyle selectedStyle, out Vector2 size)
        {
            normalStyle = new GUIStyle();
            normalStyle.normal.background = EditorGUIUtility.Load(normalBackground) as Texture2D;
            normalStyle.border = new RectOffset(border, border, border, border);
            normalStyle.alignment = TextAnchor.MiddleCenter;

            selectedStyle = new GUIStyle();
            selectedStyle.normal.background = EditorGUIUtility.Load(selectedBackground) as Texture2D;
            selectedStyle.border = new RectOffset(border, border, border, border);
            selectedStyle.alignment = TextAnchor.MiddleCenter;
            size = this.size;
        }
    }

    public static class NodeStyles
    {
        public static NodeViewStyle rootStyle = new NodeViewStyle(
            "builtin skins/darkskin/images/node1.png",
            "builtin skins/darkskin/images/node1 on.png",
            12,
            new Vector2(100, 50));

        public static NodeViewStyle standard1 = new NodeViewStyle(
            "builtin skins/darkskin/images/node2.png",
            "builtin skins/darkskin/images/node2 on.png",
            12,
            new Vector2(200, 50));

        public static NodeViewStyle standard2 = new NodeViewStyle(
            "builtin skins/darkskin/images/node3.png",
            "builtin skins/darkskin/images/node3 on.png",
            12,
            new Vector2(200, 50));

        public static NodeViewStyle standard3 = new NodeViewStyle(
            "builtin skins/darkskin/images/node4.png",
            "builtin skins/darkskin/images/node4 on.png",
            12,
            new Vector2(200, 50));

        public static NodeViewStyle standard4 = new NodeViewStyle(
            "builtin skins/darkskin/images/node5.png",
            "builtin skins/darkskin/images/node5 on.png",
            12,
            new Vector2(200, 50));

        public static NodeViewStyle redPreset = new NodeViewStyle(
            "builtin skins/darkskin/images/node6.png",
            "builtin skins/darkskin/images/node6 on.png",
            12,
            new Vector2(200, 50));
    }
}
