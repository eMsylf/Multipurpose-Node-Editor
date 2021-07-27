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
        [HideInInspector]
        public bool isSceneReference;

        public TypedVariable(int id)
        {
            name = "New " + typeof(T).Name;
            this.ID = id;
        }

        public static TypedVariable<T> CopyData(TypedVariable<T> typedVariable)
        {
            TypedVariable<T> newVariable = new TypedVariable<T>(typedVariable.ID)
            {
                name = typedVariable.name,
                value = typedVariable.value
            };
            return newVariable;
        }
    }
}