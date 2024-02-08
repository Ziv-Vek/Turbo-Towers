using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehavioralTree
{
    /**
    * This node selects a child node based on the weight of the node.
    */
    public class WeightBasedSelector : BTNode
    {
        private List<(BTNode node, float weight)> weightedNodes;
        private float totalWeight;

        public WeightBasedSelector(List<(BTNode node, float weight)> weightedNodes)
        {
            this.weightedNodes = weightedNodes;
            totalWeight = weightedNodes.Sum(n => n.weight);
        }

        public override bool Execute()
        {
            float pick = Random.value * totalWeight;
            float cumulative = 0.0f;
            foreach (var (node, weight) in weightedNodes)
            {
                cumulative += weight;
                if (pick <= cumulative)
                {
                    return node.Execute();
                }
            }

            return false;
        }
    }
}