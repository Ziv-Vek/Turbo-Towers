using TurboTowers.Turrets.Common;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBodyPart : SharedVariable<BodyPart>
    {
        public static explicit operator SharedBodyPart(BodyPart value) { return new SharedBodyPart { mValue = value }; }
    }
}