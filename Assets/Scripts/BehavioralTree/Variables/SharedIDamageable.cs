using System.Collections.Generic;
using TurboTowers.Turrets.Common;

namespace BehaviorDesigner.Runtime
{
    /** Variable of IDamageable */
    [System.Serializable]
    public class SharedIDamageable : SharedVariable<IDamageable>
    {
        public static SharedIDamageable FromIDamageable(IDamageable value)
        { return new SharedIDamageable { Value = value }; }
    }    
}