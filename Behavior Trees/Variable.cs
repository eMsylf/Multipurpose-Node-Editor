using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public class Variable<T>
    {
        public string name = "New property";
        public T value;
    }
}