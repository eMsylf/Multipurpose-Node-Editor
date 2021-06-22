using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    public class Sequence : Composite
    {
        public override Result Tick()
        {
            for (; i < children.Count; i++)
            {
                Result result = children[i].Tick();
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