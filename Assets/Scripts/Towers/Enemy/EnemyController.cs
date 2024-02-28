using System;
using System.Collections;
using BehaviorDesigner.Runtime;
using TurboTowers.Core;
using TurboTowers.Map;
using TurboTowers.Map.Models;
using UnityEngine;
using TurboTowers.Turrets.Common;

namespace TurboTowers.Turrets.Controls
{
    [RequireComponent(typeof(Health))]
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Transform baseTransform;

        private const float IntervalBetweenTargetUnpainting = 2f;
        
        private MapManager mapManager;

        private void OnEnable()
        {
            GameManager.OnGameStateChanged += OnGameStateChanged;
        }

        private void OnDisable()
        {
            GameManager.OnGameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState newGameState)
        {
            if (newGameState == GameState.InGame)
            {
                GetComponent<BehaviorTree>().EnableBehavior();
            }
            else
            {
                GetComponent<BehaviorTree>().DisableBehavior();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            //Gizmos.DrawWireSphere(baseTransform.position, 55f);
        }
        
    }
}
