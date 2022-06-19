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
        public PolygonCollider2D SpawnArea => spawnArea;

        private void Awake()
        {
            var eventBus = GameObject.FindObjectsOfType<TownPlazaAreaController>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TownPlazaAreaController can be active!");
            }

            Instance = this;
        }
    }
}