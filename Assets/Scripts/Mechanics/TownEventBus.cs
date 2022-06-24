namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TownEventBus : MonoBehaviour
    {
        public static TownEventBus Instance { get; private set; }
        public event Action<int, int> OnDayEnd;
        public event Action<int, int> OnDayStart;
        public event Action<int, int> OnActionTaken;
        public event Action<CultistObedienceLevelEnum> OnObedienceLevelChange;
        public event Action<NpcController> OnCultistJoin;
        public event Action<NpcController> OnCultistLeave;
        public event Action<float, int> OnTreeGrowthChange;
        public event Action<IEnumerable<string>> OnObjectiveUpdate;

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
            if (OnDayEnd != null)
            {
                OnDayEnd.Invoke(currentWeek, currentDay);
            }
        }
        
        public void DispatchOnDayStart(int currentWeek, int currentDay)
        {
            if (OnDayStart != null)
            {
                OnDayStart.Invoke(currentWeek, currentDay);
            }
        }

        public void DispatchOnActionTaken(int taken, int max)
        {
            if (OnActionTaken != null)
            {
                OnActionTaken.Invoke(taken, max);
            }
        }

        public void DispatchOnObedienceLevelChange(CultistObedienceLevelEnum obedienceLevel)
        {
            if (OnObedienceLevelChange != null)
            {
                OnObedienceLevelChange.Invoke(obedienceLevel);
            }
        }

        public void DispatchOnCultistJoin(NpcController npc)
        {
            if (OnCultistJoin != null)
            {
                OnCultistJoin.Invoke(npc);
            }
        }

        public void DispatchOnCultistLeave(NpcController npc)
        {
            if (OnCultistLeave != null)
            {
                OnCultistLeave.Invoke(npc);
            }
        }
        
        public void DispatchOnTreeGrowthChange(float value, int stage)
        {
            if (OnTreeGrowthChange != null)
            {
                OnTreeGrowthChange.Invoke(value, stage);
            }
        }

        public void DispatchOnObjectiveUpdate(IEnumerable<string> objectives)
        {
            if (OnObjectiveUpdate != null)
            {
                OnObjectiveUpdate.Invoke(objectives);
            }
        }
    }
}