using System;

namespace BehavioralTree
{
    /**
    * This node executes an action and returns true.
    */
    public class ActionNode : BTNode
    {
        private Action action;

        public ActionNode(Action action)
        {
            this.action = action;
        }

        public override bool Execute()
        {
            action();
            return true;
        }
    }
}