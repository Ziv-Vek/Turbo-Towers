using UnityEngine;

namespace Map.Models
{
    public class MapPoint
    {
        public Vector3 Position { get; set; }
        public PointType Type { get; set; }

        public MapPoint(Vector3 position, PointType type)
        {
            Position = position;
            Type = type;
        }
    }
}