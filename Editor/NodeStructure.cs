using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NodeEditor
{
    public class NodeStructure : ScriptableObject
    {
        public List<Node> nodes = new List<Node>();
        public List<Connection> connections = new List<Connection>();

        public NodeStructure Copy()
        {
            return (NodeStructure)MemberwiseClone();
        }

        [OnOpenAsset()]
        public static bool Open(int instanceID, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (asset.GetType() == typeof(NodeStructure))
            {
                NodeBasedEditor editor = NodeBasedEditor.OpenWindow();
                editor.Load(asset as NodeStructure);
                string name = EditorUtility.InstanceIDToObject(instanceID).name;
                Debug.Log("Open " + name);
            }
            //Debug.Log("Line: " + line);
            return false;
        }
    }
}