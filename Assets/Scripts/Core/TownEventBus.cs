namespace Horticultist.Scripts.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TownEventBus : MonoBehaviour
    {
        public static TownEventBus Instance { get; private set; }
        public event Action<int, int> OnDayEnd;
        public event Action<int, int> OnDayStart;
        public event Action<int, int> OnActionTaken;

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<TownEventBus>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TownEventBus can be active!");
            }

            Instance = this;
        }

        public void DispatchOnDayEnd(int currentWeek, int currentDay)
        {
            OnDayEnd.Invoke(currentWeek, currentDay);
        }
        
        public void DispatchOnDayStart(int currentWeek, int currentDay)
        {
            OnDayStart.Invoke(currentWeek, currentDay);
        }

        public void DispatchOnActionTaken(int taken, int max)
        {
            OnActionTaken.Invoke(taken, max);
        }
    }
}