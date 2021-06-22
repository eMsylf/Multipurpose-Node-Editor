namespace BobJeltes.AI.BehaviorTree
{
    public class Selector : Composite
    {
        public override Result Tick()
        {
            for (; i < children.Length; i++)
            {
                Result result = children[i].Tick();
                switch (result)
                {
                    case Result.Success:
                        i = 0;
                        return result;
                    case Result.Failure:
                        break;
                    case Result.Running:
                        return result;
                    default:
                        break;
                }
            }
            i = 0;
            return Result.Failure;
        }
    }
}