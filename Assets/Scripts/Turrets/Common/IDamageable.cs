using System;
using TurboTowers.Turrets.Common;
using UnityEngine;

public interface IDamageable
{
    public Health GetTurretHealth();
    public void HandleHit(int damage, Action<int> onHit);
    public Quaternion GetRotation();
    public Vector3 GetPosition();
}