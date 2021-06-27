using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class NodeStructure : ScriptableObject
    {
        //public List<Vector2> nodePositions = new List<Vector2>();
        //public List<Int2> connectionIndices = new List<Int2>();
        public List<Node> nodes = new List<Node>();

    }
}
