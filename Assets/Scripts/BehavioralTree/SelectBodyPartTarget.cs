using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Turbo Towers")]
[TaskDescription("Selects a body part to target within the enemy")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
public class SelectBodyPartTarget : Action
{
    public SharedBodyPartList bodyParts;
    public SharedBodyPart targetBodyPart;
    
    public override TaskStatus OnUpdate()
    {
        if (bodyParts.Value.Count == 0) return TaskStatus.Failure;
        var rand = Random.Range(0, bodyParts.Value.Count);
        targetBodyPart.Value = bodyParts.Value[rand];
        if (targetBodyPart == null) return TaskStatus.Failure;
        
        return TaskStatus.Success;
    }
    
    
    
}