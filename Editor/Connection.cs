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

        public Connection()
        {

        }

        public Connection (Node inNode, Node outNode, Action<Connection> OnClickRemoveConnection)
        {
            this.inNode = inNode;
            this.outNode = outNode;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw(Orientation orientation)
        {
            Draw(inNode.inPoint.rect.center, outNode.outPoint.rect.center, orientation, true);
        }

        public void Draw(Vector2 from, Vector2 to, Orientation orientation, bool hasRemoveButton)
        {
            Vector2 dir;
            switch (orientation)
            {
                default:
                case Orientation.LeftRight:
                    dir = Vector2.left;
                    break;
                case Orientation.TopBottom:
                    dir = -Vector2.up;
                    break;
            }

            Handles.DrawBezier(
                from,
                to,
                from + dir * 50f,
                to - dir * 50f,
                Color.white,
                null,
                2f);

            if (hasRemoveButton && Handles.Button((inNode.rect.center + outNode.rect.center) * .5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }
    }
}