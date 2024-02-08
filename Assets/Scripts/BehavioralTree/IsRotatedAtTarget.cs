using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using TurboTowers.Map;
using TurboTowers.Turrets.Common;

[TaskCategory("Turbo Towers")]
[TaskDescription("Checks if the agent is rotated (yawed) at the target")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
public class IsRotatedAtTarget : Conditional
{
    public SharedBodyPart target;
    public float angleThreshold = 5f;
    public SharedTransform head;
    
    public override void OnStart()
    {
        // get 
        
        
    }

    public override TaskStatus OnUpdate()
    {
        Debug.Log(head.Value.gameObject.name);
        Vector3 targetDirection = target.Value.transform.position - transform.position;
        float angle = Vector3.Angle(targetDirection, transform.forward);
        return angle < angleThreshold ? TaskStatus.Success : TaskStatus.Failure;
        
        return TaskStatus.Success;
    }
}
