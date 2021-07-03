/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using BobJeltes.AI.BehaviorTree;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    [Serializable]
    public class NodeView
    {
        public Node node;

        public Rect rect;
        public string title = "Node";
        public bool isDragged;
        private bool isSelected;
        public bool multiSelecting;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        public GUIStyle GetStyle()
        {
            return IsSelected ?
                selectedStyle 
                : 
                regularStyle;
        }

        public bool IsSelected { get => isSelected; set { if (Selectable) isSelected = value; } }

        public GUIStyle regularStyle;
        public GUIStyle selectedStyle;

        public bool Selectable;
        public Action<NodeView> OnClickNode;
        public Action<NodeView> OnClickUp;
        public Action<NodeView> OnRemoveNode;
        public Action<NodeView> OnDragNode;

        public static Dictionary<Orientation, Vector2> connectionPointSizeByOrientation = new Dictionary<Orientation, Vector2>
        {
            {Orientation.LeftRight, new Vector2(10f, 20f) },
            {Orientation.TopBottom, new Vector2(20f, 15f) }
        };

        public NodeView(Rect rect, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Orientation orientation, Action<NodeView, ConnectionPointType> onClickConnectionPoint, Action<NodeView> onClickNode, Action<NodeView> OnClickRemoveNode, Action<NodeView> onDragNode, Action<NodeView> onClickUp, Node node)
        {
            this.node = node;
            if (this.node != null) node.PositionOnView = rect.position;
            this.rect = rect;
            Type nodeType = node.GetType();
            this.title = node.GetType().Name;
            this.regularStyle = nodeStyle;
            this.selectedStyle = selectedStyle;
            if (node == null || typeof(NodeInterfaces.IHasInPort).IsAssignableFrom(nodeType))
            {
                inPoint = new ConnectionPoint(ConnectionPointType.In, connectionPointSizeByOrientation[orientation], inPointStyle, onClickConnectionPoint);
            }
            if (node == null || typeof(NodeInterfaces.IHasOutPort).IsAssignableFrom(nodeType))
            {
                outPoint = new ConnectionPoint(ConnectionPointType.Out, connectionPointSizeByOrientation[orientation], outPointStyle, onClickConnectionPoint);
            }
            if (node == null || typeof(NodeInterfaces.IInteractable).IsAssignableFrom(nodeType))
            {
                this.OnClickNode = onClickNode;
                this.OnClickUp = onClickUp;
                this.OnDragNode = onDragNode;
                this.OnRemoveNode = OnClickRemoveNode;
                Selectable = true;
            }
        }

        public NodeView(Rect rect, GUIStyle nodeStyle, GUIStyle selectedStyle, ConnectionPoint inPoint, ConnectionPoint outPoint, Action<NodeView> onClickNode, Action<NodeView> OnClickRemoveNode, Action<NodeView> onDragNode, Action<NodeView> onClickUp, bool selectable)
        {
            this.rect = rect;
            regularStyle = nodeStyle;
            this.selectedStyle = selectedStyle;
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            OnRemoveNode = OnClickRemoveNode;
            OnDragNode = onDragNode;
            OnClickNode = onClickNode;
            OnClickUp = onClickUp;
            Selectable = selectable;
        }

        public void Draw(Orientation direction)
        {
            inPoint?.Draw(direction, this);
            outPoint?.Draw(direction, this);
            GUI.Box(rect, title, GetStyle());
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (rect.Contains(e.mousePosition))
                        {
                            Click();
                            e.Use();
                        }
                    }

                    if (e.button == 1 && IsSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (e.button == 0 && rect.Contains(e.mousePosition))
                    {
                        ClickUp();
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && IsSelected)
                    {
                        Drag(e.delta, true);
                        e.Use();
                        return true;
                    }
                    break;
                case EventType.KeyDown:
                    if (e.keyCode == KeyCode.LeftControl)
                        multiSelecting = true;
                    break;
                case EventType.KeyUp:
                    if (e.keyCode == KeyCode.LeftControl)
                        multiSelecting = false;
                    break;
                default:
                    break;
            }
            return false;
        }

        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.ShowAsContext();
        }

        private void Click()
        {
            isDragged = true;
            GUI.changed = true;
            OnClickNode?.Invoke(this);
        }

        private void ClickUp()
        {
            //Debug.Log("Click up on node " + rect.position);
            isDragged = false;
            OnClickUp?.Invoke(this);
        }

        private void OnClickRemoveNode()
        {
            OnRemoveNode?.Invoke(this);
        }

        public void Drag(Vector2 delta, bool invokeCallbacks)
        {
            SetPosition(rect.position + delta);
            if (invokeCallbacks)
                OnDragNode?.Invoke(this);
        }

        public void SetPosition(Vector2 position)
        {
            rect.position = position;
            node.PositionOnView = new Vector2(position.x, position.y);
        }
    }
}