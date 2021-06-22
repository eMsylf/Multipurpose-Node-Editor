using System.Collections;
using System.Collections.Generic;

namespace BobJeltes.AI.BehaviorTree
{
    public abstract class Composite : Node
    {
        public List<Node> children;
        public int i = 0;

        public override Node Clone()
        {
            Composite node = (Composite)MemberwiseClone();
            node.children = children.ConvertAll(c => c.Clone());
            return base.Clone();
        }
    }
}