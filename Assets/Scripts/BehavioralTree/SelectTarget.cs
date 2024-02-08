using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using TurboTowers.Map;

[TaskCategory("Turbo Towers")]
[TaskDescription("Selects a target for the agent")]
[TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}CanSeeObjectIcon.png")]
public class SelectTarget : Action
{
    public SharedHealth targetableHealth;
    public SharedBodyPartList bodyParts = null;
    
    [BehaviorDesigner.Runtime.Tasks.Tooltip("If true, will only target the single player. If false, will target all players.")]
    public bool isTargetingOnlySinglePlayer;

    public override TaskStatus OnUpdate()
    {
        var playerData = MapManager.Instance.GetSinglePlayer();

        if (playerData.bodyParts == null)
        {
            Debug.Log("damageables is null for " + gameObject.name);
            return TaskStatus.Failure;
        }

        if (!playerData.isFound) return TaskStatus.Failure;

        targetableHealth.SetValue(playerData.targetHealth);
        bodyParts.SetValue(playerData.bodyParts);

        return TaskStatus.Success;
    }
}
