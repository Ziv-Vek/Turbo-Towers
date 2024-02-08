using System;
using System.Collections;
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


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            //Gizmos.DrawWireSphere(baseTransform.position, 55f);
        }
        
    }
}
