using System;

namespace BehavioralTree
{
    /**
     * This node executes a condition and returns the result.
     */
    public class Condition : BTNode
    {
        private Func<bool> condition;

        public Condition(Func<bool> condition)
        {
            this.condition = condition;
        }

        public override bool Execute()
        {
            return condition();
        }
    }
}