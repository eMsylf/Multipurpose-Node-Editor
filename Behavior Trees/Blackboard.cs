using System;
using System.Collections.Generic;
using UnityEngine;

namespace BobJeltes.AI.BehaviorTree
{
    [Serializable]
    public class Blackboard
    {
        public List<Variable<int>> integers = new List<Variable<int>>();
        public List<Variable<float>> floats = new List<Variable<float>>();
        public List<Variable<string>> strings = new List<Variable<string>>();
        public List<Variable<GameObject>> prefabs = new List<Variable<GameObject>>();
        public List<Variable<Vector3>> vector3s = new List<Variable<Vector3>>();
    }
}
