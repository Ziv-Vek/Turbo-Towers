using Map.Models;
using UnityEngine;

namespace Map
{
    public interface IMapManager
    {
        public void RegisterPoint(Vector2 position, PointType type);
    }
}