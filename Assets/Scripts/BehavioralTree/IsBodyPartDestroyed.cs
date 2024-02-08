using BehaviorDesigner.Runtime;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using TurboTowers.Map;

[TaskCategory("Turbo Towers")]
[TaskDescription("Checks if the targeted body part is destroyed.")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]

public class IsBodyPartDestroyed : Conditional
{
    public override TaskStatus OnUpdate()
    {
        
        return TaskStatus.Success;
    }
}