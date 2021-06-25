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
            BehaviorTree behaviorTree = reference as BehaviorTree;
            nodeViews = new List<NodeView>();

            if (behaviorTree == null || behaviorTree.root == null)
            {
                CreateNodeView(new RootNode(), GetDefaultRootPosition());
                return;
            }
            CreateNodeView(behaviorTree.root.Clone(), GetDefaultRootPosition());

            for (int i = 0; i < behaviorTree.nodes.Count; i++)
            {
                CreateNodeView(behaviorTree.nodes[i].Clone(), behaviorTree.nodePositions[i], false);
            }
        }

        // This does NOT add typed nodes.
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

        protected override void OnClickBackgroundWithConnection()
        {
            ProcessContextMenu(Event.current.mousePosition);
        }

        public override void Save()
        {
            if (!hasUnsavedChanges) return;
            if (reference == null) reference = CreateInstance<BehaviorTree>();
            BehaviorTree behaviorTree = reference as BehaviorTree;
            behaviorTree.nodePositions = new List<Vector2>();
            behaviorTree.nodes = new List<Node>();
            Vector2 rootNodePosition = new Vector2();

            foreach (var nodeView in nodeViews)
            {
                if (nodeView.node == null)
                {
                    UnityEngine.Debug.LogError("Node at index " + nodeViews.IndexOf(nodeView) + " is null");
                    continue;
                }
                // Root node is skipped
                if (nodeView.node.GetType() == typeof(RootNode))
                {
                    rootNodePosition = nodeView.rect.position;
                    behaviorTree.root = (RootNode)nodeView.node.Clone();
                    List<NodeView> rootNodeViewChildren = GetChildren(nodeView);
                    UnityEngine.Debug.Log("Root node has " + rootNodeViewChildren.Count + " children");
                    if (rootNodeViewChildren.Count != 0)
                        behaviorTree.root.SetChild(rootNodeViewChildren[0].node.Clone());
                    continue;
                }
                // Bug: The root's child node is cloned just above. Then the next node is cloned right below here. This causes two node clones of the same node to be created.
                // Solution: clone the entire list from the start and work with that list as a parallel to the node view's list
                behaviorTree.nodePositions.Add(rootNodePosition + nodeView.rect.position);
                behaviorTree.nodes.Add(nodeView.node.Clone());

                // Add children to nodes that have them
                Type nodeType = nodeView.node.GetType();
                List<NodeView> childNodeViews = GetChildren(nodeView);
                if (typeof(NodeInterfaces.IMultipleConnection).IsAssignableFrom(nodeType))
                {
                    List<Node> childNodes = new List<Node>();
                    foreach (var childNodeView in childNodeViews)
                    {
                        childNodes.Add(childNodeView.node.Clone());
                    }
                    (nodeView.node as NodeInterfaces.IMultipleConnection).SetChildren(childNodes);
                }
                else if (typeof(NodeInterfaces.ISingleConnection).IsAssignableFrom(nodeType)) 
                {
                    if (childNodeViews.Count != 0)
                        (nodeView.node as NodeInterfaces.ISingleConnection).SetChild(childNodeViews[0].node.Clone());
                }
            }
            UnityEngine.Debug.Log("Nodes saved: " + behaviorTree.nodes.Count);
            base.SaveChanges();
        }

        public override bool Load(NodeStructure structure)
        {
            if (!UnsavedChangesCheck()) return false;
            if (structure == null)
            {
                NewFile();
                return true;
            }
            BehaviorTree tree = structure as BehaviorTree;

            nodeViews = new List<NodeView>();
            connections = new List<Connection>();
            UnityEngine.Debug.Log("Found " + tree.nodes.Count + " nodes");
            UnityEngine.Debug.Log("Found " + tree.nodePositions.Count + " node positions");
            // Create a root node view from a new root node
            CreateNodeView(tree.root.Clone(), GetDefaultRootPosition(), false);
            // Look for connections by children in behavior tree
            // Root node has a fixed position and so the position does not need to be saved
            // ...probably
            for (int i = 0; i < tree.nodes.Count; i++)
            {
                Node node = tree.nodes[i];
                CreateNodeView(node.Clone(), tree.nodePositions[i], false);
            }
            // Iterate over nodes 
            foreach (var nodeView in nodeViews)
            {
                // Find connections
                Type nodeType = nodeView.node.GetType();
                if (!typeof(NodeInterfaces.IHasOutPort).IsAssignableFrom(nodeType))
                {
                    // Node has no outport. Continue to next one to find children to make connetions for.
                    continue;
                }
                if (typeof(NodeInterfaces.ISingleConnection).IsAssignableFrom(nodeType)) 
                {
                    // Connect the single child
                    Node childNode = (nodeView.node as NodeInterfaces.ISingleConnection).GetChild();
                    if (childNode != null)
                    {
                        // TODO: It can be that it goes wrong in this function.
                        CreateConnection(nodeViews.Find(n => n.node.guid == nodeView.node.guid), nodeViews.Find(n => n.node.guid == childNode.guid));
                        UnityEngine.Debug.Log("Create connection");
                    }
                }
                if (typeof(NodeInterfaces.IMultipleConnection).IsAssignableFrom(nodeType))
                {
                    // Connect all children
                    List<Node> children = (nodeView.node as NodeInterfaces.IMultipleConnection).GetChildren();
                    foreach (var childNode in children)
                    {
                        CreateConnection(nodeViews.Find(n => n.node.guid == nodeView.node.guid), nodeViews.Find(n => n.node.guid == childNode.guid));
                        UnityEngine.Debug.Log("Create connection");
                    }
                }
            }
            reference = structure;
            hasUnsavedChanges = false;
            ClearConnectionSelection();
            return true;
        }

        /// <summary>
        /// Creates a node view and adds it to the nodeViews list
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public NodeView CreateNodeView(Node node, Vector2 position, bool centered = true)
        {
            GUIStyle normalStyle;
            GUIStyle selectedStyle;
            Vector2 size;
            if (node.GetType() == typeof(RootNode))
            {
                NodeStyles.rootStyle.Load(out normalStyle, out selectedStyle, out size);
            }
            else
            {
                NodeStyles.standard1.Load(out normalStyle, out selectedStyle, out size);
            }
            if (centered)
            {
                position -= size * .5f;
            }
            NodeView nodeView = new NodeView(new Rect(position, size), normalStyle, selectedStyle, inPointStyle, outPointStyle, orientation, OnClickConnectionPoint, OnClickNode, OnClickRemoveNode, OnDragNode, OnClickUpNode, node);
            CreateNodeView(nodeView);
            return nodeView;
        }

        // NOTE: Might be confusing leaving this in instead of just doing the Clone() operation yourself.
        public Node CloneNode(NodeView nodeView)
        {
            return nodeView.node.Clone();
        }

        public Node CreateNode(string type)
        {
            return CreateNode(Type.GetType(type));
        }

        public Node CreateNode(Type type)
        {
            if (type == typeof(RootNode))
            {
                return new RootNode();
            }
            if (type == typeof(Sequence))
            {
                return new Sequence();
            }
            if (type == typeof(Selector))
            {
                return new Selector();
            }
            if (type == typeof(Repeat))
            {
                return new Repeat();
            }
            if (type == typeof(AI.BehaviorTree.Debug))
            {
                return new AI.BehaviorTree.Debug();
            }
            if (type == typeof(Wait))
            {
                return new Wait();
            }
            return null;
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