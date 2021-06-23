using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

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

        public static Dictionary<string, Type> stringTypeLookup;
        public static bool GetNodeType(string typeName, out Type nodeType)
        {
            // Update dictionary
            stringTypeLookup = new Dictionary<string, Type>();
            TypeCache.TypeCollection nodeTypes = TypeCache.GetTypesDerivedFrom<Node>();
            foreach (var type in nodeTypes)
            {
                stringTypeLookup.Add(type.Name, type);
            }
            if (!stringTypeLookup.ContainsKey(typeName))
            {
                Debug.LogError(typeName + " is an invalid node type");
                nodeType = typeof(Node);
                return false;
            }
            nodeType = stringTypeLookup[typeName];
            return true;
        }

        public void AddNode(string type)
        {
            if (GetNodeType(type, out Type nodeType))
            {

            }
            //nodes.Add();
            //Node node = new Type(type) as Node;
            //node.name = type.Name;
            //node.guid = GUID.Generate().ToString();
            //nodes.Add(node);

            //return node;
        }

        public void RemoveNode(Node node)
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
