using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [CreateAssetMenu(fileName = "New behavior tree", menuName = "AI/Behavior Tree")]
    public class BehaviorTree : NodeStructure
    {
        public List<BTNode> nodes;
        public Result state;
        public Blackboard blackboard;
        public RootNode root;

        // TODO: Call this from NodeBasedEditor
        public Action<List<Vector2>, List<Int2>, List<BTNode>> onSave;

        public void Save(List<BTNode> nodes, List<Int2> connections)
        {
            
        }

        public void Tick()
        {
            switch (state)
            {
                case Result.Success:
                    break;
                case Result.Failure:
                    break;
                case Result.Running:
                    state = root.Tick();
                    break;
            }
        }
    }
}
