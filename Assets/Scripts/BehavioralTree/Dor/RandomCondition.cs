using UnityEngine;

namespace BehavioralTree
{
    public class RandomCondition : BTNode
    {
        // The threshold is the probability of returning true
        private float threshold;

        public RandomCondition(float threshold)
        {
            this.threshold = threshold;
        }

        public override bool Execute()
        {
            return Random.value < threshold; // Random.value returns a random number between 0.0 and 1.0
        }
    }
}