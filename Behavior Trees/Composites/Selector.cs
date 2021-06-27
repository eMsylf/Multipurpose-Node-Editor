namespace BobJeltes.AI.BehaviorTree
{
    public class Selector : Composite
    {
        public override void OnStart()
        {
            i = 0;
        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate()
        {
            for (; i < children.Count; i++)
            {
                Result result = children[i].Update();
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
