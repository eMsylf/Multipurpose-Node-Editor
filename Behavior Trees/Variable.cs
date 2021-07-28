using System;
using System.Reflection.Emit;

namespace BobJeltes.AI.BehaviorTrees
{
    [Serializable]
    public class Variable
    {
        public string name = "New property";
        public int id;
        public int ID { get => id; set => id = value; }
        public Variable()
        {

        }

        public static TypedVariable<T> Create<T>(int id)
        {
            TypedVariable<T> variable = new TypedVariable<T>(id);
            return variable;
        }
    }
}