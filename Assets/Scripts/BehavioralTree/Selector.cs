using System.Collections.Generic;

namespace BehavioralTree
{
    /**
     * This node executes all of its children in order.
     * If any of the children returns false, this node returns false.
     * If all of the children return true, this node returns true.
     */
    public class Selector : BTNode
    {
        private List<BTNode> children;

        public Selector(List<BTNode> children)
        {
            this.children = children;
        }

        public override bool Execute()
        {
            foreach (var child in children)
            {
                if (child.Execute())
                    return true;
            }

            return false;
        }
    }
}