/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using UnityEngine;

namespace NodeEditor
{
    public enum ConnectionPointType { In, Out }
    [Serializable]
    public class ConnectionPoint
    {
        public Rect rect;
        Vector2 originalSize;
        public ConnectionPointType type;
        public GUIStyle style;
        public Action<Node, ConnectionPointType> OnClickConnectionPoint;

        public ConnectionPoint(ConnectionPointType type, Vector2 size, GUIStyle style, Action<Node, ConnectionPointType> onClickConnectionPoint)
        {
            this.type = type;
            this.style = style;
            rect = new Rect(0, 0, size.x, size.y);
            originalSize = rect.size;
            OnClickConnectionPoint = onClickConnectionPoint;
        }

        // TODO: Add
        // [DONE] top-to-bottom variant
        // (and bottom-to-top)
        // (and right-to-left?)
        public void Draw(Direction direction, Node node, float zoom)
        {
            switch (direction)
            {
                case Direction.LeftRight:
                    rect.y = node.rect.y + (node.rect.height * .5f) - rect.height * .5f;

                    switch (type)
                    {
                        case ConnectionPointType.In:
                            rect.x = node.rect.x - rect.width + 8f;
                            break;
                        case ConnectionPointType.Out:
                            rect.x = node.rect.x + node.rect.width - 8f;
                            break;
                        default:
                            break;
                    }
                    break;
                case Direction.TopBottom:
                    rect.x = node.rect.x + (node.rect.width* .5f) - rect.width * .5f;

                    switch (type)
                    {
                        case ConnectionPointType.In:
                            rect.y = node.rect.y - rect.height + 8f;
                            break;
                        case ConnectionPointType.Out:
                            rect.y = node.rect.y + node.rect.height - 8f;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            rect.size = originalSize * zoom;
            if (GUI.Button(rect, "", style))
            {
                OnClickConnectionPoint?.Invoke(node, type);
            }
        }
    }
}
