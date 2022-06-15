namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TownPlazaGameController : MonoBehaviour
    {
        [Header("Action Properties")]
        [SerializeField] private int maxAction;
        private int weekNumber = 1;
        private int dayNumber = 1;
        private int actionTaken;

        private void Start() {
            TownEventBus.Instance.DispatchOnDayStart(weekNumber, dayNumber);
        }

        public void GoNextDay()
        {
            TownEventBus.Instance.DispatchOnDayEnd(weekNumber, dayNumber);
            dayNumber += 1;
            if(dayNumber > 7)
            {
                weekNumber += 1;
                dayNumber = 1;
            }
            TownEventBus.Instance.DispatchOnDayStart(weekNumber, dayNumber);
        }
    }
}