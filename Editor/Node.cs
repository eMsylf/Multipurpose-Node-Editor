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
        public GUIStyle style;
        public GUIStyle regularStyle;
        public GUIStyle selectedStyle;

        public Action<Node, bool> OnSelectNode;
        public Action<Node> OnRemoveNode;
        public Action<Node> OnDragNode;

        public Node(Vector2 position, Vector2 size, Direction direction, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<Node, bool> onSelect, Action<Node, ConnectionPointType> OnClickConnectionPoint, Action<Node> OnClickRemoveNode, Action<Node> onDragNode)
        {
            rect = new Rect(position, size);
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
            inPoint = new ConnectionPoint(ConnectionPointType.In, connectionPointDimensions, inPointStyle, OnClickConnectionPoint);
            outPoint = new ConnectionPoint(ConnectionPointType.Out, connectionPointDimensions, outPointStyle, OnClickConnectionPoint);
            regularStyle = nodeStyle;
            this.selectedStyle = selectedStyle;
            style = regularStyle;
            OnRemoveNode = OnClickRemoveNode;
            OnDragNode = onDragNode;
            OnSelectNode = onSelect;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
            OnDragNode.Invoke(this);
        }
        // OPTION: node has only rect,  
        public void Draw(Direction direction, float zoom)
        {
            inPoint.Draw(direction, this, zoom);
            outPoint.Draw(direction, this, zoom);
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
                            Select(true);
                            style = selectedStyle;
                            e.Use();
                            GUI.changed = true;
                        }
                        else
                        {
                            if (!multiSelecting)
                            {
                                Select(false);
                                style = regularStyle;
                            }
                            GUI.changed = true;
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
                    //if (e.button == 0)
                    //{
                    //    //Drag(e.delta);
                    //    e.Use();
                    //    return true;
                    //}
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

        private void Select(bool selected)
        {
            isSelected = selected;
            OnSelectNode.Invoke(this, selected);
        }

        private void OnDrag()
        {

        }
    }
}