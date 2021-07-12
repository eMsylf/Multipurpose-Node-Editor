using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "AI/Behavior Tree")]
    public class BehaviorTree : NodeStructure
    {
        public static readonly string baseFolder = "Assets/Resources/Behavior Trees";
        public RootNode root;
        public Result result = Result.Running;
        public Blackboard blackboard = new Blackboard();
        
        public List<Node> deletedNodes = new List<Node>();

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
            if (type != typeof(RootNode))
            {
                nodes.Add(node);
            }
            else
            {
                UnityEngine.Debug.Log("Create root node");
                root = node as RootNode;
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            deletedNodes.Add(node);
            nodes.Remove(node);
        }

        public void SaveTo(BehaviorTree treeFile, string name)
        {
            bool newFile = false;
            if (treeFile == null)
            {
                // Create a new file with a unique asset path
                if (string.IsNullOrWhiteSpace(name)) name = "New Behavior Tree";
                string folderPath = FileUtility.EnsureFolderIsInAssets("Resources", "Behavior Trees");
                string completePath = folderPath + name + ".asset";
                string uniquePath = AssetDatabase.GenerateUniqueAssetPath(completePath);
                AssetDatabase.CreateAsset(this, uniquePath);
                treeFile = this;
                if (root == null)
                {
                    UnityEngine.Debug.LogError("Root is null");
                }
                AssetDatabase.AddObjectToAsset(root, treeFile);
                newFile = true;
            }
            else
            {
                //if (treeFile.root == null)
                //{
                //    // TODO: This does not belong here. Move elsewhere
                //    treeFile.root = (RootNode)CreateNode(typeof(RootNode));
                //    AssetDatabase.AddObjectToAsset(treeFile.root, treeFile);
                //}
                UnityEngine.Debug.Log("Saving to existing file");
            }

            // Clean deleted nodes list
            for (int i = 0; i < treeFile.deletedNodes.Count; i++)
            {
                if (treeFile.deletedNodes[i] == null)
                {
                    UnityEngine.Debug.LogError("Delete null node at " + i);
                    treeFile.deletedNodes.RemoveAt(i);
                    i--;
                }
            }

            // Delete node subassets that match deleted nodes
            foreach (var node in deletedNodes)
            {
                // Find the node's GUID among the tree file's nodes
                Node nodeInFile = treeFile.nodes.Find(n => n.guid == node.guid);
                if (nodeInFile != null)
                {
                    treeFile.DeleteNode(nodeInFile);
                    // If the tree file has the node object in its asset, remove it
                    if (AssetDatabase.GetAssetPath(node).Contains(AssetDatabase.GetAssetPath(treeFile)))
                    {
                        AssetDatabase.RemoveObjectFromAsset(nodeInFile);
                    }
                }
            }
            deletedNodes = new List<Node>();
            treeFile.deletedNodes = new List<Node>();

            if (newFile)
            {
                foreach (var node in nodes)
                {
                    AssetDatabase.AddObjectToAsset(node, treeFile);
                    continue;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(AssetDatabase.GetAssetPath(root)))
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

                // Go through all of the behavior tree's nodes and add them to the asset if they have not been added yet
                foreach (var node in nodes)
                {
                    // Try and find a node with the same GUID in the tree file
                    Node nodeAsset = treeFile.nodes.Find(n => n.guid == node.guid);
                    if (nodeAsset == null)
                    {
                        // If not present, add it to the tree file
                        treeFile.nodes.Add(node);
                        AssetDatabase.AddObjectToAsset(node, treeFile);
                    }
                }
            }

            // Match all the nodes with children with the children in the file
            // First, the root node
            if (root.GetChild() == null)
            {
                treeFile.root.SetChild(null);
            }
            else
            {
                treeFile.root.SetChild(nodes.Find(n => n.guid == root.GetChild().guid));
            }

            // Then every other node
            foreach (Node nodeAsset in treeFile.nodes)
            {
                if (nodeAsset.GetType().IsAssignableFrom(typeof(NodeInterfaces.IHasOutPort)))
                {
                    // Does not have out port, so has no children.
                    continue;
                }

                if (nodeAsset.GetType().IsAssignableFrom(typeof(NodeInterfaces.IMultipleConnection)))
                {
                    var multipleChildrenNode = (nodeAsset as NodeInterfaces.IMultipleConnection);
                    List<Node> childNodes = multipleChildrenNode.GetChildren();
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
                }
                else if (nodeAsset.GetType().IsAssignableFrom(typeof(NodeInterfaces.ISingleConnection)))
                {
                    var singleChildNode = (nodeAsset as NodeInterfaces.ISingleConnection);
                    Node childNode = singleChildNode.GetChild();
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
        }

        public void CleanDeletedNodes()
        {

        }

        public BehaviorTree Clone()
        {
            BehaviorTree tree = Instantiate(this);
            if (tree.root == null) tree.root = (RootNode)CreateNode(typeof(RootNode));
            tree.root = (RootNode)tree.root.Clone();
            return tree;
        }
    }
}
