using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTrees.Nodes
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

        public override Result OnUpdate(BehaviorTreeExecutor behaviorTreeExecutor)
        {
            UnityEngine.Debug.Log("Update: " + message);
            return Result.Success;
        }
    }
}
