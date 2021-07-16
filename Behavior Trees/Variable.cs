using System;
using System.Reflection.Emit;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public abstract class Variable
    {
        public string name = "New property";
        public Variable()
        {

        }

        //public static Variable Create(Type type)
        //{
        //    Variable variable = new Variable();
        //    TypeCode typeCode = System.Type.GetTypeCode(type);
        //    //Create<typeCode>();
        //    return variable;
        //}

        public static TypedVariable<T> Create<T>()
        {
            TypedVariable<T> variable = new TypedVariable<T>();
            return variable;
        }
    }
}