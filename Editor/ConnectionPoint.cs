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
        public ConnectionPointType type;
        [HideInInspector]
        // TODO: Fix looping reference
        internal Node node;
        public GUIStyle style;
        public Action<ConnectionPoint> OnClickConnectionPoint;

        public ConnectionPoint(Node node, ConnectionPointType type, Vector2 size, GUIStyle style, Action<ConnectionPoint> onClickConnectionPoint)
        {
            this.type = type;
            this.node = node;
            this.style = style;
            OnClickConnectionPoint = onClickConnectionPoint;
            rect = new Rect(0, 0, size.x, size.y);
        }

        // TODO: Add
        // [DONE] top-to-bottom variant
        // (and bottom-to-top)
        // (and right-to-left?)
        public void Draw(Direction direction)
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

            if (GUI.Button(rect, "", style))
            {
                OnClickConnectionPoint?.Invoke(this);
            }
        }
    }
}
