using UnityEditor;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class Node : ScriptableObject
    {
        private string guid;
        private Vector2 positionOnView;
        private bool started;
        private Result result;

        public Vector2 PositionOnView { get => positionOnView; set => positionOnView = value; }
        public string Guid { get => guid; set => guid = value; }
        public Result Result { get => result; set => result = value; }

        public abstract void OnStart();
        public abstract void OnStop();
        public abstract Result OnUpdate();
        public Result Update()
        {
            if (!started)
            {
                OnStart();
                started = true;
            }

            Result = OnUpdate();

            if (Result == Result.Failure || Result == Result.Success)
            {
                OnStop();
                started = false;
            }
            return Result;
        }
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