using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Wait : ActionNode
    {
        [Min(0f)]
        public float duration = 1f;
        private float startTime = 0f;

        public override void OnStart()
        {
            startTime = Time.time;
        }

        public override void OnStop()
        {

        }

        public override Result OnUpdate()
        {
            if (Time.time - startTime > duration)
                return Result.Success;
            return Result.Running;
        }
    }
}
