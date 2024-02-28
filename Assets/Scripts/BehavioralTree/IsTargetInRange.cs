using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Turbo Towers")]
[TaskDescription("Checks if the target is in range of the turret. Will run as long as the target is out of range. Will return Success when in range, and Failure if target turned null (e.g. destroyed.")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}WithinDistanceIcon.png")]
public class IsTargetInRange : Conditional
{
    public SharedBodyPart target;
    public SharedTransform myTurret;
    public SharedWeaponStatsSO weaponStatsSO;

    public override TaskStatus OnUpdate()
    {
        if (target.Value == null)
        {
            return TaskStatus.Failure;
        }
        
        Vector3 offset = target.Value.transform.position - myTurret.Value.position;
        float offsetSqrMagnitude = offset.sqrMagnitude;

        if (offsetSqrMagnitude < weaponStatsSO.Value.GetBaseRange(0) * weaponStatsSO.Value.GetBaseRange(0))
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}
