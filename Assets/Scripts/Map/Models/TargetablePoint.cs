using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Map.Models
{
    [System.Serializable]
    public class TargetablePoint
    {
        [ShowInInspector] public Vector3 BasePosition { get; set; }
        [ShowInInspector] public PointType Type { get; set; }
        [ShowInInspector] [CanBeNull] public List<IDamageable> DamageableParts { get; set; }
        
        public TargetablePoint(PointType type,
            Vector3 basePosition,
            [CanBeNull] List<IDamageable> damageableParts)
        {
            Type = type;
            BasePosition = basePosition;
            DamageableParts = damageableParts;
        }
    }

    public class TowerPoint
    {
        [ShowInInspector] public Vector3 BasePosition { get; set; }
        [ShowInInspector] public PointType Type { get; set; }
        [ShowInInspector] [CanBeNull] public List<BodyPart> BodyParts { get; set; }
        
        public TowerPoint(PointType type,
            Vector3 basePosition,
            [CanBeNull] List<BodyPart> bodyParts)
        {
            Type = type;
            BasePosition = basePosition;
            BodyParts = bodyParts;
        }
        
        public void RemoveBodyPart(BodyPart bodyPart)
        {
            Debug.Log(BodyParts!.Remove(bodyPart));
        }
        
        public void AddBodyPart(BodyPart bodyPart)
        {
            BodyParts!.Insert(BodyParts.Count - 1, bodyPart);
        }

        public void RemoveAllParts()
        {
            BodyParts!.Clear();
        }
    }
}