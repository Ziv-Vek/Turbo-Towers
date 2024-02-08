using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedWeaponStatsSO : SharedVariable<WeaponStatsSO>
    {
        public static explicit operator SharedWeaponStatsSO(WeaponStatsSO value) { return new SharedWeaponStatsSO { mValue = value }; }
    }
}


