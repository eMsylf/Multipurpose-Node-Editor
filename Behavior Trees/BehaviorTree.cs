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
        public Blackboard blackboard;

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
            nodes.Add(node);

            return node;
        }

        public void DeleteNode(Node node)
        {
            nodes.Remove(node);
        }

        public void Save(string name = "Behavior Tree")
        {
            string ownAssetPath = AssetDatabase.GetAssetPath(this);
            // Create a behavior tree asset if not yet present
            if (string.IsNullOrWhiteSpace(ownAssetPath))
            {
                string folder = EnsureFolderIsInAssets("Resources", "Behavior Trees");
                string uniquePath = AssetDatabase.GenerateUniqueAssetPath(folder + name + ".asset");
                AssetDatabase.CreateAsset(this, uniquePath);
                UnityEngine.Debug.Log("File saved at " + uniquePath, this);
            }

            string rootNodeAssetPath = AssetDatabase.GetAssetPath(Root);
            if (string.IsNullOrWhiteSpace(rootNodeAssetPath) || !rootNodeAssetPath.Contains(ownAssetPath)) 
            {
                AssetDatabase.AddObjectToAsset(Root, this);
            }

            // Go through all of the behavior tree's nodes
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
                    // Node already added to behavior tree. Update the node at this path with the data of this one
                    UnityEngine.Debug.Log(node.name + " node already added to " + name);
                }
            }
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a folder in the Assets folder
        /// </summary>
        /// <param name="folders">Cascading subfolders. For example: "Characters", "Materials"</param>
        /// <returns>The folder path including the last forward slash (/). For example: Assets/Characters/Materials/</returns>
        public static string EnsureFolderIsInAssets(params string[] folders)
        {
            string completeFolderPath = "Assets";
            for (int i = 0; i < folders.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(completeFolderPath + "/" + folders[i]))
                {
                    AssetDatabase.CreateFolder(completeFolderPath, folders[i]);
                    UnityEngine.Debug.Log("Folder created: " + completeFolderPath);
                }

                completeFolderPath += "/" + folders[i];
            }
            return completeFolderPath + "/";
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            tree.Root = (RootNode)tree.Root.Clone();
            return tree;
        }
    }
}
