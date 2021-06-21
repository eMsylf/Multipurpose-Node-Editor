using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NodeEditor
{

    public class NodeStructure : ScriptableObject
    {
        public List<Vector2> nodePositions = new List<Vector2>();
        public List<Int2> connectionIndices = new List<Int2>();

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