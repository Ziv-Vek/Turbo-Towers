using BehaviorDesigner.Runtime;

namespace BehaviorDesigner.Runtime
{
    /** Variable of Itargetable */
    [System.Serializable]
    public class SharedITargetable : SharedVariable<ITargetable>
    {
        public static SharedITargetable FromITargetable(ITargetable value)
        { return new SharedITargetable { mValue = value }; }

        // public static SharedITargetable FromITargetable(ITargetable value)
        // { return new SharedITargetable { Value = value }; }
    }    
}
