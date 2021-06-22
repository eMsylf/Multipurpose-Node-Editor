namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Composite : BTNode
    {
        public BTNode[] children;
        public int i = 0;
    }
}