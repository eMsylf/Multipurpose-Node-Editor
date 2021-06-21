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
    }
}
