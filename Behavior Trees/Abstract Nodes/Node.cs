using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class Node : ScriptableObject
    {
        public string guid;
        public Vector2 positionOnView;
        private bool started;
        public Result result;

        public abstract void OnStart();
        public abstract void OnStop();
        public abstract Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor);
        public Result BTUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            result = OnUpdate(behaviorTreeExecutor);

            if (result == Result.Failure || result == Result.Success)
            {
                OnStop();
                started = false;
            }
            return result;
        }
        
        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public virtual void CopyData(Node source)
        {
            positionOnView = source.positionOnView;
        }
    }

    public enum Result
    {
        Success,
        Failure,
        Running
    }
}
