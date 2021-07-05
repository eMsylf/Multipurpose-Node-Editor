using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Sequence : Composite
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
                    case Result.Failure:
                        i = 0;
                        return result;
                    case Result.Running:
                        return result;
                }
            }
            i = 0;
            return Result.Success;
        }
    }
}
