namespace Horticultist.Scripts.Extensions
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public static class ColliderExtensions 
    {
        public static Vector2 GetRandomPoint(this PolygonCollider2D area)
        {
            var point = Vector2.zero;
            do
            {
                point = new Vector2(
                    Random.Range(area.bounds.min.x, area.bounds.max.x),
                    Random.Range(area.bounds.min.y, area.bounds.max.y)
                );
            } while (!area.OverlapPoint(point));

            return point;
        }
    }
}