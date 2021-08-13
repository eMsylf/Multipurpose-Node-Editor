using BobJeltes.AI.BehaviorTrees.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "Bob Jeltes/AI/Behavior Trees/Behavior Tree", order = 82)]
    public class BehaviorTree : NodeStructure
    {
        public static readonly string baseFolder = "Assets/Resources/Behavior Trees";
        public RootNode root;
        public Result result = Result.Running;
        [Tooltip("Use this to manually edit your blackboard variables")]
        public Blackboard blackboard = new Blackboard();
        public void AddVariable(Type type)
        {
            Undo.RecordObject(this, "Add variable");
            blackboard.AddVariable(type);
            EditorUtility.SetDirty(this);
            OnBehaviorTreeAltered?.Invoke();
        }
        public void RemoveVariable<T>(TypedVariable<T> variable)
        {
            Undo.RecordObject(this, "Remove variable");
            blackboard.RemoveVariable(variable);
            EditorUtility.SetDirty(this);
            OnBehaviorTreeAltered?.Invoke();
        }

        private List<Node> deletedNodes = new List<Node>();

        public Result BTUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            switch (result)
            {
                case Result.Running:
                    result = root.BTUpdate(behaviorTreeExecutor);
                    break;
            }
            return result;
        }

        public Node CreateNode(Type type)
        {
            Node node = CreateInstance(type) as Node;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();
            if (type != typeof(RootNode))
            {
                nodes.Add(node);
            }
            else
            {
                //UnityEngine.Debug.Log("Create root node");
                root = node as RootNode;
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            OnBehaviorTreeAltered?.Invoke();

            return node;
        }

        public void DeleteNode(Node node)
        {
            deletedNodes.Add(node);
            nodes.Remove(node);
            OnBehaviorTreeAltered?.Invoke();
        }

        public BehaviorTree SaveToNew(string name)
        {
            BehaviorTree treeFile;

            // Create a new file with a unique asset path
            if (string.IsNullOrWhiteSpace(name)) name = "New Behavior Tree";
            string folderPath = FileUtility.EnsureFolderIsInAssets("Resources", "Behavior Trees");
            string completePath = folderPath + name + ".asset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(completePath);
            AssetDatabase.CreateAsset(this, uniquePath);
            EditorUtility.SetDirty(this);
            treeFile = this;
            if (root == null)
            {
                UnityEngine.Debug.LogError("Root is null");
                root = CreateInstance<RootNode>();
            }
            AssetDatabase.AddObjectToAsset(root, treeFile);

            foreach (var node in nodes)
            {
                AssetDatabase.AddObjectToAsset(node, treeFile);
                continue;
            }

            SaveTo(ref treeFile);
            return treeFile;
        }

        public void SaveTo(ref BehaviorTree treeFile)
        {
            treeFile.root.CopyData(root);

            // Go through all of the behavior tree's nodes and add them to the asset if they have not been added yet
            foreach (var node in nodes)
            {
                // Try and find a node with the same GUID in the tree file
                int nodeIndex = treeFile.nodes.FindIndex(n => n.guid == node.guid);
                if (nodeIndex == -1)
                {
                    // If not present, add it to the tree file
                    treeFile.nodes.Add(node);
                    AssetDatabase.AddObjectToAsset(node, treeFile);
                }
                else
                {
                    // If present, update
                    treeFile.nodes[nodeIndex].CopyData(node);
                }
            }

            UnityEngine.Object[] nodeAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(treeFile));
            for (int i = 0; i < nodeAssets.Length; i++)
            {
                UnityEngine.Debug.Log("File found: " + nodeAssets[i].name);
                if (nodeAssets[i].GetType() == typeof(RootNode))
                {
                    UnityEngine.Debug.Log("Skip root node");
                    continue;
                }

                Node nodeAsset = nodeAssets[i] as Node;

                if (nodeAsset == null)
                {
                    UnityEngine.Debug.Log("Could not convert to node?");
                    continue;
                }
                else
                {
                    UnityEngine.Debug.Log("IT IS A NODE! YAY!");
                }

                if (nodes.Exists(node => node.guid == nodeAsset.guid))
                {
                    UnityEngine.Debug.Log(nodeAssets[i] + " found in nodes list. Skipping...");
                    continue;
                }

                UnityEngine.Debug.Log(nodeAssets[i].name + " no longer exists.");
                AssetDatabase.RemoveObjectFromAsset(nodeAssets[i]);
                treeFile.DeleteNode(nodeAsset);
            }

            // Create a root node if not yet present. Update it if there is
            if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(treeFile.root)))
            {
                if (treeFile.root != null)
                {
                    AssetDatabase.AddObjectToAsset(treeFile.root, treeFile);
                }
                else
                {
                    AssetDatabase.AddObjectToAsset(root, treeFile);
                }
            }


            // Match all the nodes with children with the children in the file
            // First, the root node
            if (root.GetChild() == null) treeFile.root.SetChild(null);
            else treeFile.root.SetChild(treeFile.nodes.Find(n => n.guid == root.GetChild().guid));

            // Then every other node
            foreach (Node nodeAsset in treeFile.nodes)
            {
                var multipleChildrenNode = nodeAsset as NodeInterfaces.IMultipleConnection;
                if (multipleChildrenNode != null)
                {
                    List<Node> childNodes = multipleChildrenNode.GetChildren();
                    childNodes = childNodes.OrderBy(x => x.positionOnView.x).ToList();
                    for (int i = 0; i < childNodes.Count; i++)
                    {
                        Node childNode = childNodes[i];
                        Node childNodeInFile = treeFile.nodes.Find(n => n.guid == childNode.guid);
                        if (childNodeInFile != null)
                        {
                            // Override the node reference
                            childNodes[i] = childNodeInFile;
                        }
                        else
                        {
                            AssetDatabase.AddObjectToAsset(childNode, treeFile);
                        }
                    }
                    // Order the children by the position on the x axis to enforce left-to-right execution
                    // TODO: Implement different ordering criterium for left-to-right-oriented trees?
                    childNodes.OrderBy(n => n.positionOnView.x);
                    continue;
                }
                var singleChildNode = nodeAsset as NodeInterfaces.ISingleConnection;
                if (singleChildNode != null)
                {
                    Node childNode = singleChildNode.GetChild();
                    if (childNode == null) continue;
                    Node childNodeInFile = treeFile.nodes.Find(n => n.guid == childNode.guid);
                    if (childNodeInFile != null)
                    {
                        // Override the node reference
                        singleChildNode.SetChild(childNodeInFile);
                    }
                    else
                    {
                        AssetDatabase.AddObjectToAsset(childNode, treeFile);
                    }
                }
            }
            AssetDatabase.SaveAssets();
            OnBehaviorTreeAltered?.Invoke();
        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            for (int i = 0; i < tree.nodes.Count; i++)
            {
                tree.nodes[i] = tree.nodes[i].Clone();
            }
            if (tree.root == null) tree.root = (RootNode)CreateNode(typeof(RootNode));
            tree.root = (RootNode)tree.root.Clone();
            return tree;
        }

        public Action OnBehaviorTreeAltered;
    }
}
