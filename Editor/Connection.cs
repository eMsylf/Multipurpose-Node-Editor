/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class Connection
    {
        public ConnectionPoint inPoint;
        public ConnectionPoint outPoint;
        public Action<Connection> OnClickRemoveConnection;

        public Connection (ConnectionPoint inPoint, ConnectionPoint outPoint, Action<Connection> OnClickRemoveConnection)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw(Direction direction)
        {
            Vector2 dir;
            switch (direction)
            {
                default:
                case Direction.LeftRight:
                    dir = Vector2.left;
                    break;
                case Direction.TopBottom:
                    dir = -Vector2.up;
                    break;
            }

            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + dir * 50f,
                outPoint.rect.center - dir * 50f,
                Color.white,
                null,
                2f);

            if (Handles.Button((inPoint.rect.center + outPoint.rect.center) * .5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }


    }
}