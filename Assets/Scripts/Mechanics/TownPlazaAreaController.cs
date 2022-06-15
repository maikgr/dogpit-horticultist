namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TownPlazaAreaController : MonoBehaviour
    {
        public static TownPlazaAreaController Instance { get; private set; }

        [SerializeField] private PolygonCollider2D spawnArea;
        [SerializeField] private PolygonCollider2D excludedArea;

        private void Awake()
        {
            var eventBus = GameObject.FindObjectsOfType<TownPlazaAreaController>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TownPlazaAreaController can be active!");
            }

            Instance = this;
        }

        public Vector2 GetRandomPoint()
        {
            var point = new Vector2(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
            );
            while (!spawnArea.OverlapPoint(point) || excludedArea.OverlapPoint(point))
            {
                point = new Vector2(
                    Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                    Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
                );
            }

            return point;
        }
    }
}