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

        public static int uniqueID = int.MinValue;
        public int GetUniqueID()
        {
            int value = uniqueID;
            uniqueID++;
            return value;
        }

        public Blackboard()
        {

        }

        public Blackboard(Blackboard original)
        {
            Blackboard clone = (Blackboard)original.MemberwiseClone();
            bools = clone.bools;
            ints = clone.ints;
            floats = clone.floats;
            strings = clone.strings;
            Colors = clone.Colors;
            Vector2s = clone.Vector2s;
            Vector3s = clone.Vector3s;
            Vector4s = clone.Vector4s;
            Vector2Ints = clone.Vector2Ints;
            Vector3Ints = clone.Vector3Ints;
            GameObjects = clone.GameObjects;
            masterList = clone.masterList;
        }

        public void AddVariable(Type type)
        {
            Variable createdVariable = null;
            if (type == typeof(bool))
            {
                createdVariable = Variable.Create<bool>(GetUniqueID());
                bools.Add((TypedVariable<bool>)createdVariable);
            }
            else if (type == typeof(int))
            {
                createdVariable = Variable.Create<int>(GetUniqueID());
                ints.Add((TypedVariable<int>)createdVariable);
            }
            else if (type == typeof(float))
            {
                createdVariable = Variable.Create<float>(GetUniqueID());
                floats.Add((TypedVariable<float>)createdVariable);
            }
            else if (type == typeof(string))
            {
                createdVariable = Variable.Create<string>(GetUniqueID());
                strings.Add((TypedVariable<string>)createdVariable);
            }
            else if (type == typeof(Color))
            {
                createdVariable = Variable.Create<Color>(GetUniqueID());
                Colors.Add((TypedVariable<Color>)createdVariable);
            }
            else if (type == typeof(Vector2))
            {
                createdVariable = Variable.Create<Vector2>(GetUniqueID());
                Vector2s.Add((TypedVariable<Vector2>)createdVariable);
            }
            else if (type == typeof(Vector3))
            {
                createdVariable = Variable.Create<Vector3>(GetUniqueID());
                Vector3s.Add((TypedVariable<Vector3>)createdVariable);
            }
            else if (type == typeof(Vector4))
            {
                createdVariable = Variable.Create<Vector4>(GetUniqueID());
                Vector4s.Add((TypedVariable<Vector4>)createdVariable);
            }
            else if (type == typeof(Vector2Int))
            {
                createdVariable = Variable.Create<Vector2Int>(GetUniqueID());
                Vector2Ints.Add((TypedVariable<Vector2Int>)createdVariable);
            }
            else if (type == typeof(Vector3Int))
            {
                createdVariable = Variable.Create<Vector3Int>(GetUniqueID());
                Vector3Ints.Add((TypedVariable<Vector3Int>)createdVariable);
            }
            else if (type == typeof(GameObject))
            {
                createdVariable = Variable.Create<GameObject>(GetUniqueID());
                GameObjects.Add((TypedVariable<GameObject>)createdVariable);
            }

            if (masterList == null) masterList = new List<Variable>();
            if (createdVariable != null)
            {
                masterList.Add(createdVariable);
            }
        }

        public Variable GetVariable(int id)
        {
            return masterList.Find(x => x.ID == id);
        }

        public GameObject GetGameObjectVariable(int id)
        {
            var variable = GameObjects.Find(x => x.ID == id);
            if (variable == null) return null;
            return variable.value;
        }

        public void RemoveVariable<T>(TypedVariable<T> variable)
        {
            switch (variable)
            {
                case TypedVariable<bool> boolVar:
                    bools.Remove(boolVar);
                    break;
                case TypedVariable<int> intVar:
                    ints.Remove(intVar);
                    break;
                case TypedVariable<float> floatVar:
                    floats.Remove(floatVar);
                    break;
                case TypedVariable<string> stringVar:
                    strings.Remove(stringVar);
                    break;
                case TypedVariable<Color> ColorVar:
                    Colors.Remove(ColorVar);
                    break;
                case TypedVariable<Vector2> Vector2Var:
                    Vector2s.Remove(Vector2Var);
                    break;
                case TypedVariable<Vector3> Vector3Var:
                    Vector3s.Remove(Vector3Var);
                    break;
                case TypedVariable<Vector4> Vector4Var:
                    Vector4s.Remove(Vector4Var);
                    break;
                case TypedVariable<Vector2Int> Vector2IntVar:
                    Vector2Ints.Remove(Vector2IntVar);
                    break;
                case TypedVariable<Vector3Int> Vector3IntVar:
                    Vector3Ints.Remove(Vector3IntVar);
                    break;
                case TypedVariable<GameObject> GameObjectVar:
                    GameObjects.Remove(GameObjectVar);
                    break;
                default:
                    return;
            }

            masterList.Remove(variable);
        }
    }
}
