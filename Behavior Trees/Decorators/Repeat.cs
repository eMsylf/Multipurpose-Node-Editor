using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Repeat : Decorator
    {
        public override void OnStart()
        {
            
        }

        public override void OnStop()
        {
            
        }

        public override Result OnUpdate()
        {
            Child.Update();
            return Result.Running;
        }
    }
}
