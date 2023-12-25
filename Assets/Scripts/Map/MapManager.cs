using System.Collections.Generic;
using Map.Models;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour, IMapManager
    {
        private readonly Dictionary<Vector2, MapPoint> _mapPoints = new();

        public void RegisterPoint(Vector2 position, PointType type)
        {
            _mapPoints[position] = new MapPoint(position, type);
        }

        public void UnRegisterPoint(Vector2 position)
        {
            _mapPoints.Remove(position);
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

        public (bool found, Vector2 closestEmptyPoint) GetClosestEmptyPoint(Vector2 position)
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
        }

        public (bool found, Vector2 closestEnemyPoint) GetClosestEnemyPoint(Vector2 position)
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
        }
    }
}