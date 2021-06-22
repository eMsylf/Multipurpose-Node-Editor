using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "AI/Behavior Tree")]
    public class BehaviorTree : NodeStructure
    {
        public List<BTNode> nodes;
        public Blackboard blackboard;
        public RootNode root;

        // TODO: Call this from NodeBasedEditor
        public Action<List<Vector2>, List<Int2>, List<BTNode>> onSave;

        public void Tick()
        {
            root.Tick();
        }
    }
}