using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public class TypedVariable<T> : Variable
    {
        public T value;

        public TypedVariable(int id)
        {
            name = "New " + typeof(T).Name;
            this.ID = id;
        }
    }
}