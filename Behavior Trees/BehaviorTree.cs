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
                    root = CreateInstance<RootNode>(); 
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
                    result = root.Update();
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

        public void Save()
        {
            string ownAssetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrWhiteSpace(ownAssetPath))
            {
                string folder = EnsureFolderIsInAssets("Resources", "Behavior Trees");
                string uniquePath = AssetDatabase.GenerateUniqueAssetPath(folder + "Test asset" + ".asset");
                AssetDatabase.CreateAsset(this, uniquePath);
                UnityEngine.Debug.Log("File saved at " + uniquePath, this);
            }
            foreach (var node in nodes)
            {
                string assetPath = AssetDatabase.GetAssetPath(node);
                // Check if the node is already added to the node. If so, do nothing.
                if (assetPath == null || assetPath.Contains(ownAssetPath))
                {
                    // Node already added to behavior tree. Do nothing.
                }
                else
                {
                    // The node has not been added to the asset, so add it to the asset.
                    AssetDatabase.AddObjectToAsset(node, this);
                }
            }
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Creates a folder in the Assets folder
        /// </summary>
        /// <param name="folders">Cascading subfolders. For example: "Characters", "Materials"</param>
        /// <returns>The folder path including the last forward slash (/). For example: Assets/Characters/Materials/</returns>
        public string EnsureFolderIsInAssets(params string[] folders)
        {
            string completeFolderPath = "Assets";
            for (int i = 0; i < folders.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(completeFolderPath + "/" + folders[i])) 
                    AssetDatabase.CreateFolder(completeFolderPath, folders[i]);
                completeFolderPath += "/" + folders[i];
            }
            UnityEngine.Debug.Log("Folder created: " + completeFolderPath);
            return completeFolderPath + "/";
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = (BehaviorTree)MemberwiseClone();
            tree.root = (RootNode)tree.root.Clone();
            return tree;
        }
    }
}