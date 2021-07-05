using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Repeat : Decorator
    {
        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            
        }

        protected override Result OnUpdate()
        {
            Child.Update();
            return Result.Running;
        }
    }
}
