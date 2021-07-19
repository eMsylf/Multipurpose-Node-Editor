using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public class Blackboard
    {
        public List<TypedVariable<bool>> bools = new List<TypedVariable<bool>>();
        public List<TypedVariable<int>> ints = new List<TypedVariable<int>>();
        public List<TypedVariable<float>> floats = new List<TypedVariable<float>>();
        public List<TypedVariable<string>> strings = new List<TypedVariable<string>>();
        public List<TypedVariable<Color>> Colors = new List<TypedVariable<Color>>();
        public List<TypedVariable<Vector2>> Vector2s = new List<TypedVariable<Vector2>>();
        public List<TypedVariable<Vector3>> Vector3s = new List<TypedVariable<Vector3>>();
        public List<TypedVariable<Vector4>> Vector4s = new List<TypedVariable<Vector4>>();
        public List<TypedVariable<Vector2Int>> Vector2Ints = new List<TypedVariable<Vector2Int>>();
        public List<TypedVariable<Vector3Int>> Vector3Ints = new List<TypedVariable<Vector3Int>>();
        public List<TypedVariable<GameObject>> GameObjects = new List<TypedVariable<GameObject>>();

        //[Header("Use this to manually edit and reorder your variables")]
        public List<Variable> masterList;

        public void AddVariable(Type type)
        {
            Variable createdVariable = null;
            if (type == typeof(bool))
            {
                createdVariable = Variable.Create<bool>();
                bools.Add((TypedVariable<bool>)createdVariable);
            }
            else if (type == typeof(int))
            {
                createdVariable = Variable.Create<int>();
                ints.Add((TypedVariable<int>)createdVariable);
            }
            else if (type == typeof(float))
            {
                createdVariable = Variable.Create<float>();
                floats.Add((TypedVariable<float>)createdVariable);
            }
            else if (type == typeof(string))
            {
                createdVariable = Variable.Create<string>();
                strings.Add((TypedVariable<string>)createdVariable);
            }
            else if (type == typeof(Color))
            {
                createdVariable = Variable.Create<Color>();
                Colors.Add((TypedVariable<Color>)createdVariable);
            }
            else if (type == typeof(Vector2))
            {
                createdVariable = Variable.Create<Vector2>();
                Vector2s.Add((TypedVariable<Vector2>)createdVariable);
            }
            else if (type == typeof(Vector3))
            {
                createdVariable = Variable.Create<Vector3>();
                Vector3s.Add((TypedVariable<Vector3>)createdVariable);
            }
            else if (type == typeof(Vector4))
            {
                createdVariable = Variable.Create<Vector4>();
                Vector4s.Add((TypedVariable<Vector4>)createdVariable);
            }
            else if (type == typeof(Vector2Int))
            {
                createdVariable = Variable.Create<Vector2Int>();
                Vector2Ints.Add((TypedVariable<Vector2Int>)createdVariable);
            }
            else if (type == typeof(Vector3Int))
            {
                createdVariable = Variable.Create<Vector3Int>();
                Vector3Ints.Add((TypedVariable<Vector3Int>)createdVariable);
            }
            else if (type == typeof(GameObject))
            {
                createdVariable = Variable.Create<GameObject>();
                GameObjects.Add((TypedVariable<GameObject>)createdVariable);
            }

            if (createdVariable != null)
            {
                if (masterList == null) masterList = new List<Variable>();
                masterList.Add(createdVariable);
            }
        }

        public void RemoveVariable<T>(TypedVariable<T> variable)
        {
            switch (variable)
            {
                case TypedVariable<bool> boolVar:
                    bools.Remove(boolVar);
                    break;
                default:
                    break;
            }
        }
    }
}
