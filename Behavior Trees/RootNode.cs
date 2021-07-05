using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class RootNode : Node, NodeInterfaces.IHasOutPort, NodeInterfaces.ISingleConnection
    {
        private Node child;

        public Node Child { get => child; set => child = value; }

        public void SetChild(Node node)
        {
            Child = node;
        }

        public override Node Clone()
        {
            RootNode node = Instantiate(this);
            node.Child = Child?.Clone();
            return node;
        }

        protected override Result OnUpdate()
        {
            return Child.Update();
        }

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        public Node GetChild()
        {
            return Child;
        }
    }
}
