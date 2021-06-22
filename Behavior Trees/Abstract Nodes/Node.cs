namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class Node
    {
        public string name;
        public string guid;
        public abstract Result Tick();
        public virtual Node Clone()
        {
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