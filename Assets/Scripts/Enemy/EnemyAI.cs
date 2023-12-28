using System.Collections.Generic;
using System.Linq;
using BehavioralTree;
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
        private BTNode behaviorTree;

        private EnemyState State { get; set; } = EnemyState.Idle;

        private void Start()
        {
            behaviorTree = BuildBehaviorTree();

            _enemyController = GetComponent<EnemyController>();
            _playerRotator = GetComponent<PlayerRotator>();
            mapManager.RegisterPoint(transform.position, PointType.Enemy);
        }


        void Update()
        {
            behaviorTree.Execute();
        }

        private BTNode BuildBehaviorTree()
        {
            return new Selector(new List<BTNode>
            {
                new Sequence(new List<BTNode>
                {
                    new Condition(IsEnemyPlayerInSight),
                    new ActionNode(TargetClosestEnemyPlayer),
                    new ActionNode(AttackEnemyPlayer)
                }),
                new Sequence(new List<BTNode>
                {
                    new Condition(IsUnderThreat),
                    new ActionNode(FindNearestCover),
                    new ActionNode(MoveToCover)
                }),
                new ActionNode(Patrol)
            });
        }

        private bool IsEnemyPlayerInSight()
        {
            var enemyPlayers = mapManager.GetEnemyPlayers();
            var closestEnemyPlayer = enemyPlayers
                .OrderBy(p => Vector3.Distance(transform.position, p.Position))
                .FirstOrDefault();
            if (closestEnemyPlayer == null)
                return false;

            var direction = closestEnemyPlayer.Position - transform.position;
            var angle = Vector3.Angle(direction, transform.forward);
            if (angle < 45f)
            {
                return true;
            }

            return false;
        }

        private void TargetClosestEnemyPlayer()
        {
        }

        private void AttackEnemyPlayer()
        {
        }

        private bool IsUnderThreat()
        {
            var enemyPlayers = mapManager.GetEnemyPlayers();
            var closestEnemyPlayer = enemyPlayers
                .OrderBy(p => Vector3.Distance(transform.position, p.Position))
                .FirstOrDefault();
            if (closestEnemyPlayer == null)
                return false;

            var direction = closestEnemyPlayer.Position - transform.position;
            var angle = Vector3.Angle(direction, transform.forward);
            if (angle < 45f)
            {
                return true;
            }

            return false;
        }

        private void FindNearestCover()
        {
        }

        private void MoveToCover()
        {
        }

        private void Patrol()
        {
        }


        private void OnDestroy()
        {
            mapManager.RegisterPoint(transform.position, PointType.Empty);
        }
    }
}