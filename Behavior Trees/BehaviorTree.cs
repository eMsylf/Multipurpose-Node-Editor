using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "AI/Behavior Tree")]
    public class BehaviorTree : NodeStructure
    {
        public static readonly string baseFolder = "Assets/Resources/Behavior Trees";
        [SerializeField]
        private RootNode root;
        public RootNode Root { 
            get 
            {
                if (root == null) 
                { 
                    root = (RootNode)CreateNode(typeof(RootNode)); 
                } 
                return root; 
            }
            set => root = value;
        }
        public Result result = Result.Running;
        public Blackboard blackboard = new Blackboard();
        
        public List<Node> deletedNodes = new List<Node>();

        public Result Update()
        {
            switch (result)
            {
                case Result.Running:
                    result = Root.Update();
                    break;
            }
            return result;
        }

        public Node CreateNode(Type type)
        {
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            if (type == typeof(RootNode))
            {
                Root = (RootNode)node;
            }
            else
            {
                nodes.Add(node);
            }

            return node;
        }

        public void DeleteNode(Node node)
        {
            deletedNodes.Add(node);
            nodes.Remove(node);
        }

        public void Save()
        {
            string rootPath = AssetDatabase.GetAssetPath(Root);
            if (rootPath == null)
                AssetDatabase.AddObjectToAsset(Root, this);
            else
            {
                // TODO: Apply changes to the existing root node?
                // OR: Delete old root node and replace?
            }

            string ownAssetPath = AssetDatabase.GetAssetPath(this);
            // Go through all of the behavior tree's nodes and add them to the asset if they have not been added yet
            foreach (var node in nodes)
            {
                string nodeAssetPath = AssetDatabase.GetAssetPath(node);
                // Check if the node is already added to the node. If so, do nothing.
                if (string.IsNullOrWhiteSpace(nodeAssetPath) || !nodeAssetPath.Contains(ownAssetPath))
                {
                    // The node has not been added to the asset, so add it to the asset.
                    AssetDatabase.AddObjectToAsset(node, this);
                }
                else
                {
                    UnityEngine.Debug.Log(node.name + " node already added to " + name);
                    // Node already added to behavior tree.

                    // TODO: Update the node at this path with the data of this one

                }
            }

            foreach (var nodeAsset in deletedNodes)
            {
                // If the node asset is present, remove it from the behavior tree asset
                if (!string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(nodeAsset)))
                    AssetDatabase.RemoveObjectFromAsset(nodeAsset);
            }
            deletedNodes = new List<Node>();
            AssetDatabase.SaveAssets();
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.Root = (RootNode)tree.Root.Clone();
            return tree;
        }
    }
}
