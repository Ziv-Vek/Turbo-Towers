using System.Collections.Generic;

namespace BehavioralTree
{
    /**
   * This node executes all of its children and returns true if all of them return true.
   */
    public class Sequence : BTNode
    {
        private readonly List<BTNode> _children;

        public Sequence(List<BTNode> children)
        {
            this._children = children;
        }

        public override bool Execute()
        {
            foreach (var child in _children)
            {
                if (!child.Execute())
                    return false;
            }

            return true;
        }
    }
}