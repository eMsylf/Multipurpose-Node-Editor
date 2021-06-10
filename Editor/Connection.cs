/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class Connection
    {
        public Node inNode;
        public Node outNode;
        //public GUID inNode;
        //public GUID outNode;
        public Action<Connection> OnClickRemoveConnection;

        public Connection (Node inNode, Node outNode, Action<Connection> OnClickRemoveConnection)
        {
            this.inNode = inNode;
            this.outNode = outNode;
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
                inNode.inPoint.rect.center,
                outNode.outPoint.rect.center,
                inNode.inPoint.rect.center + dir * 50f,
                outNode.outPoint.rect.center - dir * 50f,
                Color.white,
                null,
                2f);

            if (Handles.Button((inNode.rect.center + outNode.rect.center) * .5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }
    }
}