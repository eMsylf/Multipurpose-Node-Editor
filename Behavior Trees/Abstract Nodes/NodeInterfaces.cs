using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
{
    public class NodeInterfaces : MonoBehaviour
    {
        public interface ISingleConnection
        {
            public abstract void SetChild(Node node);
            public abstract Node GetChild();
        }

        public interface IMultipleConnection
        {
            public abstract void SetChildren(List<Node> nodes);
            public abstract void AddChild(Node node);
            public abstract void RemoveChild(Node node);
            public abstract List<Node> GetChildren();
        }

        public interface IHasInPort
        {

        }

        public interface IHasOutPort
        {

        }

        public interface IInteractable
        {

        }
    }
}