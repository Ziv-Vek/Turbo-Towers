using System.Collections.Generic;
using JetBrains.Annotations;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Map
{
    public interface IMapManager
    {
        public void RegisterPoint(ITargetable targetableHealth,
            PointType type,
            Vector3 basePosition,
            [CanBeNull] List<IDamageable> damageableParts);
        public void UnRegisterPoint(Health target);
        public void RemoveDamageablePartFromPoint(Health target, BodyPart bodyPart);
        public void AddDamageablePartToPoint(Health target, BodyPart bodyPart);
    }
}