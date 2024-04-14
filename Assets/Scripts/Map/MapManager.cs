using System;
using System.Collections;
using System.Collections.Generic;
using DrawXXL;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TurboTowers.Core;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Combat;
using TurboTowers.Turrets.Common;
using UnityEngine;
using TurboTowers.Movement;

namespace TurboTowers.Map
{
    public class MapManager : MonoBehaviour, IMapManager
    {
        private static MapManager _instance;
        public static MapManager Instance => _instance;

        private readonly Dictionary<Vector2, MapPoint> _mapPoints = new();
        [ShowInInspector] public Dictionary<ITargetable, TargetablePoint> _targetPoints = new();
        [ShowInInspector] public Dictionary<Health, TowerPoint> targetTowers = new();

        private GameObject player;

        public event Action OnBossDeath;

        #region Unity Events
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        #endregion


        #region Public Methods

        public void RegisterPoint(ITargetable targetableHealth,
            PointType type,
            Vector3 basePosition,
            List<IDamageable> damageableParts)
        {
            if (type == PointType.Portal)
            {
                _targetPoints[targetableHealth] = new TargetablePoint(type, basePosition, null);
            }
            else
            {
                _targetPoints[targetableHealth] = new TargetablePoint(type, basePosition, damageableParts);
            }
        }
        
        public void RegisterTower(Health tower,
            PointType type,
            Vector3 basePosition,
            List<BodyPart> bodyParts)
        {
            if (type == PointType.Portal)
            {
                targetTowers[tower] = new TowerPoint(type, basePosition, null);
            }
            else
            {
                targetTowers[tower] = new TowerPoint(type, basePosition, bodyParts);
            }
        }

        public void UnRegisterPoint(Health target)
        {
            if (targetTowers[target].Type == PointType.Enemy)
            {
                var newPortal = PortalsManager.Instance.SpawnPortal(targetTowers[target].BasePosition);
                newPortal.DeactivatePortal();
                
                targetTowers[target].Type = PointType.Portal;
                targetTowers[target].RemoveAllParts();
                //targetTowers.Remove(target);
                
                TeleportToPortal(newPortal);
                
                if (!AreRemainingTowers()) GameManager.Instance.HandleAllTowersKilled();
            }
            
            if (targetTowers[target].Type == PointType.Player)
            {
                //TODO: Implement game over logic
            }

            if (targetTowers[target].Type == PointType.Boss)
            {
                Debug.Log("Boss killed!");
                OnBossDeath?.Invoke();
            }
        }

        /** Checks if there are any remaining towers (enemies) in the map (boss inclusive).
         * Returns - true if at least one enemy is alive, otherwise returns false */
        private bool AreRemainingTowers()
        {
            Dictionary<Health, TowerPoint>.ValueCollection pointsValues = targetTowers.Values;
            foreach (var towerPoint in pointsValues)
            {
                Debug.Log("in loop");
                Debug.Log(towerPoint.Type);
                if (towerPoint.Type == PointType.Enemy) return true;
            }
            
            return false;
        }

        public void RemoveDamageablePartFromPoint(Health targetableHealth, BodyPart damageablePartToRemove)
        {
            if (_targetPoints[targetableHealth].Type == PointType.Portal) return;
            if (_targetPoints[targetableHealth].DamageableParts == null) return;
            if (_targetPoints[targetableHealth].DamageableParts.Count == 0) return;
            
            _targetPoints[targetableHealth].DamageableParts.Remove(damageablePartToRemove);
        }
        
        public void RemoveBodyPartFromPoint(Health target, BodyPart bodyPart)
        {
            if (targetTowers[target].Type == PointType.Portal) return;
            if (targetTowers[target].BodyParts == null) return;
            if (targetTowers[target].BodyParts.Count == 0) return;
            
            targetTowers[target].RemoveBodyPart(bodyPart);
        }
        
        /**
         * Add damageable part (e.g BodyPart) to targetable point (e.g player/enemy).
         * This always adds to the end of the list, thus the list does not maintain any order (e.g is not necessarily head, body parts, body).
         */
        public void AddDamageablePartToPoint(Health target, BodyPart bodyPart)
        {
            if (targetTowers[target].Type == PointType.Portal) return;
            
            targetTowers[target].AddBodyPart(bodyPart);
        }
        
        public (bool isFound, Health targetHealth, List<BodyPart> bodyParts) GetSinglePlayer()
        {
            foreach (var targetTower in targetTowers)
            {
                if (targetTower.Value.Type == PointType.Player)
                {
                    bool isAlive = targetTower.Key.GetCurrentHealth() > 0;
                    if (!isAlive) return (false, null, null);
                    
                    return (true, targetTower.Key, targetTower.Value.BodyParts);
                }
            }

            return (false, null, null);
        }
        
        public List<MapPoint> GetEnemyPlayers()
        {
            var enemyPlayers = new List<MapPoint>();
            foreach (var mapPoint in _mapPoints.Values)
            {
                if (mapPoint.Type != PointType.Enemy) continue;
                enemyPlayers.Add(mapPoint);
            }

            return enemyPlayers;
        }

        public MapPoint GetPoint(Vector2 position)
        {
            return _mapPoints[position];
        }

        public bool IsPointEmpty(Vector2 position)
        {
            return _mapPoints[position].Type == PointType.Empty;
        }

        public bool IsPointOccupied(Vector2 position)
        {
            return _mapPoints[position].Type != PointType.Empty;
        }

        #endregion

        /*public (bool found, Vector2 closestEmptyPoint) GetClosestEmptyPoint(Vector2 position)
        {
            Vector2 closestEmptyPoint = Vector2.zero;
            float closestDistance = float.MaxValue;
            bool found = false;

            foreach (var mapPoint in _mapPoints.Values)
            {
                if (mapPoint.Type != PointType.Empty) continue;
                float distance = Vector2.Distance(position, mapPoint.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEmptyPoint = mapPoint.Position;
                    found = true;
                }
            }

            return (found, closestEmptyPoint);
        }*/

        /*public (bool found, Vector2 closestEnemyPoint) GetClosestEnemyPoint(Vector2 position)
        {
            Vector2 closestEnemyPoint = Vector2.zero;
            float closestDistance = float.MaxValue;
            bool found = false;

            foreach (var mapPoint in _mapPoints.Values)
            {
                if (mapPoint.Type != PointType.Enemy) continue;
                float distance = Vector2.Distance(position, mapPoint.Position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemyPoint = mapPoint.Position;
                    found = true;
                }
            }

            return (found, closestEnemyPoint);
        }*/
        
        /*public List<MapPoint> GetEnemyPlayers()
        {
            var enemyPlayers = new List<MapPoint>();
            foreach (var mapPoint in _mapPoints.Values)
            {
                if (mapPoint.Type != PointType.Enemy) continue;
                enemyPlayers.Add(mapPoint);
            }

            return enemyPlayers;
        }*/


        #region Private Methods  

        private void TeleportToPortal(Portal newPortal)
        {
            if (newPortal == null) return;
            
            if (player.TryGetComponent(out TeleportationController teleportationController))
            {
                StartCoroutine(teleportationController.Teleport(newPortal,true));
            }
        }

        #endregion
    }
}