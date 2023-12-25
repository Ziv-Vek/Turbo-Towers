using System.Collections.Generic;
using System.Linq;
using Enemy.Models;
using Map;
using Map.Models;
using UnityEngine;

namespace Enemy
{
    public class EnemyAI : MonoBehaviour, IEnemyAI
    {
        [SerializeField] private MapManager mapManager;
        [SerializeField] private EnemyAIType type;

        private EnemyController _enemyController;
        private PlayerRotator _playerRotator;

        private EnemyState State { get; set; } = EnemyState.Idle;

        private void Start()
        {
            _enemyController = GetComponent<EnemyController>();
            _playerRotator = GetComponent<PlayerRotator>();
            mapManager.RegisterPoint(transform.position, PointType.Enemy);
        }

        void Update()
        {
            if (State == EnemyState.KnockedOut) return;
            EnemyAction bestAction = DetermineBestAction();
            ExecuteAction(bestAction);
        }

        EnemyAction DetermineBestAction()
        {
            float attackEnemy = CalculateAttackEnemy();
            float retreat = CalculateRetreat();

            // Choose the action with the highest utility
            return new Dictionary<EnemyAction, float>
                {
                    { EnemyAction.AttackEnemy, attackEnemy },
                    { EnemyAction.Retreat, retreat }
                }
                .OrderByDescending(pair => pair.Value)
                .First().Key;
        }

        float CalculateMoveToCover()
        {
            // Example calculation: More useful when health is low and enemy is close
//        return (1 - health) * (1 / (distanceToEnemy + 1));
            return 0;
        }

        float CalculateAttackEnemy()
        {
            // Example calculation: More useful when ammo is high and enemy is close
            //  return ammo * (1 / (distanceToEnemy + 1));
            return 1;
        }

        float CalculateRetreat()
        {
            // Example calculation: More useful when health is very low
            //  return (1 - health) * 2;
            return 0;
        }

        void ExecuteAction(EnemyAction action)
        {
            switch (action)
            {
                case EnemyAction.AttackEnemy:
                    State = EnemyState.Shooting;
                    
                    break;
                case EnemyAction.Retreat:
                    State = EnemyState.Moving;
                    break;
            }

            State = EnemyState.Idle;
        }

        private void OnDestroy()
        {
            mapManager.RegisterPoint(transform.position, PointType.Empty);
        }
    }
}