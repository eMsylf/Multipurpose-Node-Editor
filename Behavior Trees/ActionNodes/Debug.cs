using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Debug : ActionNode
    {
        public string message = "";

        public override void OnStart()
        {
            UnityEngine.Debug.Log("Start: " + message);
        }

        public override void OnStop()
        {
            UnityEngine.Debug.Log("Stop: " + message);
        }

        public override Result Tick()
        {
            UnityEngine.Debug.Log("Tick: " + message);
            return Result.Success;
        }
    }
}
