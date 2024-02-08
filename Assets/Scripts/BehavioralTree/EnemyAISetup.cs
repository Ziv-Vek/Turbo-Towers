using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.SharedVariables;
using TurboTowers.Map;
using UnityEngine;

public class EnemyAISetup : Action
{
    //public SharedObject weaponStats;
    public SharedWeaponStatsSO weaponStatsSO;
    public SharedGameObject playerGO;
    public SharedBool isTargetingOnlySinglePlayer = true;

    public override void OnStart()
    {
        base.OnStart();
        
        
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}