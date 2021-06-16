/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class Node
    {
        public Rect rect;
        public string title;
        public bool isDragged;
        public bool isSelected;
        public bool multiSelecting;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        // OPTION: Remove these styles and pass them as parameters in Draw()
        // Would that be less efficient?
        public GUIStyle style { get => isSelected ? selectedStyle : regularStyle; }
        public GUIStyle regularStyle;
        public GUIStyle selectedStyle;

        public Action<Node> OnClickNode;
        public Action<Node> OnClickUp;
        public Action<Node> OnRemoveNode;
        public Action<Node> OnDragNode;

        public Node(Vector2 position, Vector2 size, Orientation direction, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<Node> onClickNode, Action<Node, ConnectionPointType> onClickConnectionPoint, Action<Node> OnClickRemoveNode, Action<Node> onDragNode, Action<Node> onClickUp)
        {
            rect = new Rect(position, size);
            Vector2 connectionPointDimensions;
            switch (direction)
            {
                default:
                case Orientation.LeftRight:
                    connectionPointDimensions = new Vector2(10f, 20f);
                    break;
                case Orientation.TopBottom:
                    connectionPointDimensions = new Vector2(20, 10);
                    break;
            }
            inPoint = new ConnectionPoint(ConnectionPointType.In, connectionPointDimensions, inPointStyle, onClickConnectionPoint);
            outPoint = new ConnectionPoint(ConnectionPointType.Out, connectionPointDimensions, outPointStyle, onClickConnectionPoint);
            regularStyle = nodeStyle;
            this.selectedStyle = selectedStyle;
            OnRemoveNode = OnClickRemoveNode;
            OnDragNode = onDragNode;
            OnClickNode = onClickNode;
            this.OnClickUp = onClickUp;
        }

        public void Draw(Orientation direction)
        {
            GUI.Box(rect, title, style);
            inPoint.Draw(direction, this);
            outPoint.Draw(direction, this);
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

                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    ClickUp();
                    e.Use();
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isSelected)
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
            OnClickNode.Invoke(this);
        }

        private void ClickUp()
        {
            isDragged = false;
            OnClickUp.Invoke(this);
        }

        private void OnClickRemoveNode()
        {
            OnRemoveNode?.Invoke(this);
        }

        public void Drag(Vector2 delta, bool invokeCallbacks)
        {
            rect.position += delta;
            if (invokeCallbacks)
                OnDragNode.Invoke(this);
        }
    }
}