using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Invertor : Decorator
    {
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override Result OnUpdate()
        {
            switch (child.result)
            {
                case Result.Success:
                    return Result.Failure;
                case Result.Failure:
                    return Result.Success;
                case Result.Running:
                    break;
            }
            return Result.Running;
        }
    }
}