using UnityEngine;
using System.Collections.Generic;
using TurboTowers.Turrets.Common;

namespace BehaviorDesigner.Runtime
{
    /** Variable of List<IDamageable>. BodyPart inherits from IDamageable. */
    [System.Serializable]
    public class SharedIDamageableList : SharedVariable<List<IDamageable>>
    {
        public SharedIDamageableList()
        {
            mValue = new List<IDamageable>();
        }
        
        public static implicit operator SharedIDamageableList(List<IDamageable> value) { return new SharedIDamageableList { mValue = value }; }
        
        /*public static SharedIDamageableList FromIDamageableList(List<IDamageable> value)
        { return new SharedIDamageableList { Value = value }; }*/
    }
}

