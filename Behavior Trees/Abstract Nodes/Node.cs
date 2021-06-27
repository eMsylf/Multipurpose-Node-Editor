using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class Node : ScriptableObject
    {
        public string guid;
        public Vector2 positionOnView;
        public abstract void OnStart();
        public abstract void OnStop();
        public abstract Result Tick();
        public virtual Node Clone()
        {
            return Instantiate(this);
        }
    }

    public enum Result
    {
        Success,
        Failure,
        Running
    }
}