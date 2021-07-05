namespace BobJeltes.AI.BehaviorTree
{
    public class Selector : Composite
    {
        protected override void OnStart()
        {
            i = 0;
        }

        protected override void OnStop()
        {

        }

        protected override Result OnUpdate()
        {
            for (; i < Children.Count; i++)
            {
                Result result = Children[i].Update();
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
