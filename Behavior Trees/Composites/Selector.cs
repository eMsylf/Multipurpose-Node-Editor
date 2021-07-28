using BobJeltes.AI.BehaviorTrees.Nodes;

namespace BobJeltes.AI.BehaviorTrees
{
    public class Selector : CompositeNode
    {
        public override void OnStart()
        {
            i = 0;
        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            for (; i < children.Count; i++)
            {
                Result result = children[i].BTUpdate(behaviorTreeExecutor);
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
