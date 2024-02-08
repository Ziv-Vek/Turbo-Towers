namespace BehavioralTree
{
    /**
 * This is the base class for all nodes in the behavioral tree.
 * The Execute method is called every frame.
 * It returns true if the node succeeded and false if it failed.
 */
    public abstract class BTNode
    {
        public abstract bool Execute();
    }
}