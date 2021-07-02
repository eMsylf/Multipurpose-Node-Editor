/// Based on the tutorial by Oguzkonya at https://oguzkonya.com/creating-node-based-editor-unity/

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.NodeEditor
{
    [Serializable]
    public class Connection
    {
        public NodeView inNodeView;
        public NodeView outNodeView;
        public Action<Connection> OnClickRemoveConnection;

        private Dictionary<Orientation, Vector2> orientationToDirection = new Dictionary<Orientation, Vector2> {
            { Orientation.LeftRight, Vector2.left},
            { Orientation.TopBottom, -Vector2.up}
        };

        public Connection()
        {

        }

        public Connection(NodeView outNode, NodeView inNode, Action<Connection> OnClickRemoveConnection)
        {
            this.inNodeView = inNode;
            this.outNodeView = outNode;
            this.OnClickRemoveConnection = OnClickRemoveConnection;
        }

        public void Draw(Orientation orientation)
        {
            Draw(inNodeView.inPoint.rect.center, outNodeView.outPoint.rect.center, orientation, true);
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

            if (hasRemoveButton && Handles.Button((inNodeView.rect.center + outNodeView.rect.center) * .5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
            {
                OnClickRemoveConnection?.Invoke(this);
            }
        }
    }
}