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
        public Vector2 originalSize;
        public string title;
        public bool isDragged;
        public bool isSelected;
        public bool multiSelecting;
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;

        private GUIStyle style;
        private GUIStyle defaultNodeStyle;
        private GUIStyle selectedNodeStyle;

        public Action<Node> OnRemoveNode;

        public Node(Vector2 position, Vector2 size, Direction direction, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<Node, ConnectionPointType> OnClickNode, Action<Node> OnClickRemoveNode)
        {
            rect = new Rect(position, size);
            originalSize = rect.size;
            style = nodeStyle;
            Vector2 connectionPointDimensions;
            switch (direction)
            {
                default:
                case Direction.LeftRight:
                    connectionPointDimensions = new Vector2(10f, 20f);
                    break;
                case Direction.TopBottom:
                    connectionPointDimensions = new Vector2(20, 10);
                    break;
            }
            inPoint = new ConnectionPoint(ConnectionPointType.In, connectionPointDimensions, inPointStyle, OnClickNode);
            outPoint = new ConnectionPoint(ConnectionPointType.Out, connectionPointDimensions, outPointStyle, OnClickNode);
            defaultNodeStyle = nodeStyle;
            selectedNodeStyle = selectedStyle;
            OnRemoveNode = OnClickRemoveNode;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw(Direction direction, float zoom)
        {
            inPoint.Draw(direction, this, zoom);
            outPoint.Draw(direction, this, zoom);
            rect.size = originalSize * zoom;
            GUI.Box(rect, title, style);
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
                            isDragged = true;
                            GUI.changed = true;
                            isSelected = true;
                            style = selectedNodeStyle;
                            e.Use();
                        }
                        else
                        {
                            GUI.changed = true;
                            if (!multiSelecting)
                            {
                                isSelected = false;
                                style = defaultNodeStyle;
                            }
                        }
                    }

                    if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                    {
                        ProcessContextMenu();
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    isDragged = false;
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0 && isDragged)
                    {
                        Drag(e.delta);
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

        private void OnClickRemoveNode()
        {
            OnRemoveNode?.Invoke(this);
        }
    }
}