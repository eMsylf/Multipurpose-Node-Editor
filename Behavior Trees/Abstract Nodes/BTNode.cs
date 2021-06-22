namespace BobJeltes.AI.BehaviorTree
{
    [System.Serializable]
    public abstract class BTNode
    {
        public abstract Result Tick();
    }

    public enum Result
    {
        Success,
        Failure,
        Running
    }
}