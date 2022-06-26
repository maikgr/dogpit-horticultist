namespace Horticultist.Scripts.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TherapyEventBus : MonoBehaviour
    {
        public static TherapyEventBus Instance { get; private set; }
        public event Action<ClutterWorkEvent> OnPatienceChanged;
        public event Action<ClutterWorkEvent> OnIndoctrinationChanged;
        public event Action<MoodEnum> OnMoodChanged;
        public event Action<NpcTypeEnum, MoodEnum> OnTherapyEnds;

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<TherapyEventBus>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TherapyEventBus can be active!");
            }

            Instance = this;
        }

        public void DispatchOnPatienceChanged(ClutterWorkEvent workEvent)
        {
            if (OnPatienceChanged!= null) 
            {
                OnPatienceChanged.Invoke(workEvent);
            }
        }
        
        public void DispatchOnIndoctrinationChanged(ClutterWorkEvent workEvent)
        {
            if (OnIndoctrinationChanged != null)
            {
                OnIndoctrinationChanged.Invoke(workEvent);
            }
        }

        public void DispatchOnMoodChanged(MoodEnum mood)
        {
            if (OnMoodChanged != null)
            {
                OnMoodChanged.Invoke(mood);
            }
            
        }

        public void DispatchOnTherapyEnds(NpcTypeEnum npcType, MoodEnum mood)
        {
            if (OnTherapyEnds != null)
            {
                OnTherapyEnds.Invoke(npcType, mood);
            }
        }
    }

    public class ClutterWorkEvent
    {
        public int amountChanged;
        public int remainingWorkTime;
        public int totalWorkTime;
    }
}