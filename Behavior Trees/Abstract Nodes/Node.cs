using UnityEditor;

namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class Node
    {
        public string name;
        public GUID guid = new GUID();
        public abstract void OnStart();
        public abstract void OnStop();
        public abstract Result Tick();
        public virtual Node Clone()
        {
            if (guid.Empty()) guid = GUID.Generate();
            return (Node)MemberwiseClone();
        }
    }

    public enum Result
    {
        Success,
        Failure,
        Running
    }
}