using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "AI/Behavior Tree")]
    public class BehaviorTree : NodeStructure
    {
        public RootNode root;
        public List<Node> nodes;
        public Result state = Result.Running;
        public Blackboard blackboard;

        // TODO: Call this from NodeBasedEditor
        public Action<List<Vector2>, List<Int2>, List<Node>> onSave;

        public void Save(List<Node> nodes, List<Int2> connections)
        {
            
        }

        public Result Tick()
        {
            switch (state)
            {
                case Result.Running:
                    state = root.Tick();
                    break;
            }
            return state;
        }

        //public Node CreateNode(Type type)
        //{
        //    Node node = new Node(type) as Node;
        //    node.name = type.Name;
        //    node.guid = GUID.Generate().ToString();
        //    nodes.Add(node);

        //    return node;
        //}

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = (BehaviorTree)MemberwiseClone();
            tree.root = (RootNode)tree.root.Clone();
            return tree;
        }
    }
}
