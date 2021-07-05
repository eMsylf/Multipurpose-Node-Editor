using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Debug : ActionNode
    {
        public string message = "";

        protected override void OnStart()
        {
            UnityEngine.Debug.Log("Start: " + message);
        }

        protected override void OnStop()
        {
            UnityEngine.Debug.Log("Stop: " + message);
        }

        protected override Result OnUpdate()
        {
            UnityEngine.Debug.Log("Update: " + message);
            return Result.Success;
        }
    }
}
