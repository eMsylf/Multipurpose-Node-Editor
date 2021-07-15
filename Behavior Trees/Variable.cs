using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public class Variable
    {
        public string name = "New property";
        [Variable]
        public UnityEngine.Object value;
        public Type variableType;
        public Variable()
        {

        }

        public static Variable Create(Type type)
        {
            Variable variable = new Variable();
            variable.variableType = type;
            return variable;
        }

        public static T Create<T>() where T: UnityEngine.Object
        {
            Variable variable = new Variable();
            variable.variableType = typeof(T);
            return variable as T;
        }
    }
}