namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.Extensions;

    public class TutorialStateVariables : MonoBehaviour
    {
        public static TutorialStateVariables Instance => _instance;
        private static TutorialStateVariables _instance;

        public bool HaveShownFirstTimePlaza { get; set; }
        public bool HaveShownFirstTimeTherapy { get; set; }
        public bool HaveShownFirstTimeWealth { get; set; }
        public bool HaveShownFirstTimeHealth { get; set; }       
        public bool HaveShownFirstTimeLove { get; set; }
        public bool HaveShownFirstSuccessfulRecruit { get; set; }
        public bool HaveShownFirstTimeSuccessfulTherapy { get; set; }
        public bool HaveShownFirstTimeFailedTherapy { get; set; }
        public bool HaveShownStartDayTwo { get; set; }
        public bool HaveShownFirstTimeScoldPraise { get; set; }
        public bool HaveShownFirstTimeSacrifice { get; set; }

        public bool CanShowFirstSuccessfulRecruit { get; set; }
        public bool CanShowFirstTimeSuccessfulTherapy { get; set; }
        public bool CanShowFirstTimeFailedTherapy { get; set; }
        public bool CanShowStartDayTwo { get; set; }

        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<GameStateController>();
            if (controllers.Length > 1)
            {
                Debug.Log("Found existing GameStateController, destroying");
                Destroy(this.gameObject);
            }

            _instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

    }
}