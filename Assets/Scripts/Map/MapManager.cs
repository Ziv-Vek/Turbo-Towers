using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using TurboTowers.Map.Models;
using TurboTowers.Turrets.Common;
using UnityEngine;

namespace TurboTowers.Map
{
    public class MapManager : MonoBehaviour, IMapManager
    {
        private static MapManager _instance;
        public static MapManager Instance => _instance;

        private readonly Dictionary<Vector2, MapPoint> _mapPoints = new();
        [ShowInInspector] public Dictionary<ITargetable, TargetablePoint> _targetPoints = new();
        [ShowInInspector] public Dictionary<Health, TowerPoint> targetTowers = new();
        
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
            if (targetTowers[target].Type == PointType.Enemy || targetTowers[target].Type == PointType.Player)
            {
                PortalsManager.Instance.SpawnPortal(targetTowers[target].BasePosition);
                
                targetTowers[target].Type = PointType.Portal;
                targetTowers[target].RemoveAllParts();
            }

            if (targetTowers[target].Type == PointType.Boss)
            {
                Debug.Log("Boss killed!");
                OnBossDeath?.Invoke();
            }
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
    }
}