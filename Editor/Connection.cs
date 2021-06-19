/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    [Serializable]
    public class Connection
    {
        public Node inNode;
        public Node outNode;
        public int inNodeIndex;
        public int outNodeIndex;
        //public GUID inNode;
        //public GUID outNode;
        public Action<Connection> OnClickRemoveConnection;

        private Dictionary<Orientation, Vector2> orientationToDirection = new Dictionary<Orientation, Vector2> {
            { Orientation.LeftRight, Vector2.left},
            { Orientation.TopBottom, -Vector2.up} 
        };

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
            Vector2 dir = orientationToDirection[orientation];

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