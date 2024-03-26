using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Common;

[TaskCategory("Turbo Towers")]
[TaskDescription("Attacks the target")]
[TaskIcon("Assets/Behavior Designer Tactical/Editor/Icons/{SkinColor}AttackIcon.png")]

public class ShootAtTarget : Action
{
    public SharedBodyPart target;
    public SharedFloat powerupSpeed = 0.1f;
    public SharedGameObject projectilePrefab;
    public SharedTransform projectileSpawnPoint;
    public SharedHealth myHealth;
    public SharedFloat firingPowerMagnitude = 5f;

    public SharedGameObject parentColliderGO;   // the nearest parent with the collider to be aware of when the projectile is fired    
    
    private bool isAvailableToFire;
    
    #region Events
    public event System.Action OnTurretPowering;
    public event System.Action OnTurretFired;
          
    public System.Action<int, PointType, BodyPartType?> OnTurretHit;
    #endregion
    
    public override void OnStart()
    {
        isAvailableToFire = true;
        OnTurretHit = HandleHit;
    }

    public override TaskStatus OnUpdate()
    {
        if (!isAvailableToFire) return TaskStatus.Running;

        if (target.Value == null) return TaskStatus.Success;
        
        var projectile = GameObject.Instantiate(projectilePrefab.Value,
            projectileSpawnPoint.Value.position,
            Quaternion.identity) as GameObject;
        //Physics.IgnoreCollision(projectile.GetComponent<Collider>(), gameObject.GetComponentInParent<Collider>());
        projectile.GetComponent<Projectile>().Fire(projectileSpawnPoint.Value.forward,
            firingPowerMagnitude.Value,
            myHealth.Value,
            OnTurretHit, parentColliderGO.Value);
        
        return TaskStatus.Success;
    }
    
    private void HandleHit(int damagePerformed, PointType pointType, BodyPartType? bodyPartType)
    {
        if (pointType == PointType.Enemy)
        {
            GetComponent<Health>().GainHealth(damagePerformed);    
        }
        
        OnTurretHit = null;
    }

    public override void OnEnd()
    {
        base.OnEnd();
        isAvailableToFire = true;
    }
}