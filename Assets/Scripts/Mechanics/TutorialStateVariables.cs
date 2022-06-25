namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TutorialStateVariables : MonoBehaviour
    {
        public static TutorialStateVariables Instance => _instance;
        private static TutorialStateVariables _instance;

        public bool IsFirstTimePlaza { get; set; }
        
        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<GameStateController>();
            if (controllers.Length > 1)
            {
                Debug.Log("Found existing GameStateController, destroying");
                Destroy(this.gameObject);
            }

            _instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);

            IsFirstTimePlaza = false;
        }

    }
}