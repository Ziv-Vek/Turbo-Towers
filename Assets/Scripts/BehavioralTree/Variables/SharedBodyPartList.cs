using UnityEngine;
using System.Collections.Generic;
using TurboTowers.Turrets.Common;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedBodyPartList : SharedVariable<List<BodyPart>>
    {
        public SharedBodyPartList()
        {
            mValue = new List<BodyPart>();
        }

        public static implicit operator SharedBodyPartList(List<BodyPart> value) { return new SharedBodyPartList { mValue = value }; }
    }
}