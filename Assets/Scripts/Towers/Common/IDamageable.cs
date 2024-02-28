using System;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Common;
using UnityEngine;

public interface IDamageable
{
    public Health GetTurretHealth();
    public void HandleHit(int damage, Action<int, PointType> onHit);
    public Quaternion GetRotation();
    public Vector3 GetPosition();
}