using TurboTowers.Turrets.Common;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedHealth : SharedVariable<Health>
    {
        public static explicit operator SharedHealth(Health value) { return new SharedHealth { mValue = value }; }
    }
}