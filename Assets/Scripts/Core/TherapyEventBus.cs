namespace Horticultist.Scripts.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TherapyEventBus : MonoBehaviour
    {
        public static TherapyEventBus Instance { get; private set; }
        public event Action<int> OnPatienceChanged;
        public event Action<int> OnIndoctrinationChanged;
        public event Action<MoodEnum> OnMoodChanged;

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<TherapyEventBus>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TherapyEventBus can be active!");
            }

            Instance = this;
        }

        public void DispatchOnPatienceChanged(int amountChanged)
        {
            if (OnPatienceChanged!= null) 
            {
                OnPatienceChanged.Invoke(amountChanged);
            }
        }
        
        public void DispatchOnIndoctrinationChanged(int amountChanged)
        {
            if (OnIndoctrinationChanged != null)
            {
                OnIndoctrinationChanged.Invoke(amountChanged);
            }
        }

        public void DispatchOnMoodChanged(MoodEnum mood)
        {
            if (OnMoodChanged != null)
            {
                OnMoodChanged.Invoke(mood);
            }
            
        }
    }
}