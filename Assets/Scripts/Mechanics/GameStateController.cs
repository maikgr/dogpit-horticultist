namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameStateController : MonoBehaviour
    {
        public static GameStateController Instance { get; private set; }
        public NpcController SelectedNpc { get; private set; }
        public bool PlayerHasConverted => ConvertCount > 0;
        public int ConvertCount { get; private set; }

        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<GameStateController>();
            if (controllers.Length > 1)
            {
                Debug.LogError("Only one GameStateController can be active!");
            }

            Instance = this;
            GameObject.DontDestroyOnLoad(this);
        }

        private void Start() {
            ConvertCount = 0;
        }

        public void SetSelectedNpc(NpcController npc)
        {
            SelectedNpc = npc;
        }

        public void AddConvertCount()
        {
            ConvertCount += 1;
        }
    }
}