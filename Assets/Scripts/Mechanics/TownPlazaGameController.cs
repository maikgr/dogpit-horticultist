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
        private int weekNumber;
        private int dayNumber;
        private int actionTaken;

        private void Start() {
            this.weekNumber = 1;
            this.dayNumber = 1;
            TownEventBus.Instance.DispatchOnDayStart(this.weekNumber, this.dayNumber);
        }

        public void EndDay()
        {
            this.actionTaken = 0;
            TownEventBus.Instance.DispatchOnActionTaken(this.actionTaken, this.maxAction);
            GoNextDay();
        }

        private void GoNextDay()
        {
            TownEventBus.Instance.DispatchOnDayEnd(this.weekNumber, this.dayNumber);
            this.dayNumber += 1;
            if(this.dayNumber > 7)
            {
                this.weekNumber += 1;
                this.dayNumber = 1;
            }
            TownEventBus.Instance.DispatchOnDayStart(this.weekNumber, this.dayNumber);
        }

        public void AddAction()
        {
            this.actionTaken += 1;
            if (this.actionTaken >= this.maxAction)
            {
                this.actionTaken = 0;
                GoNextDay();
            }
            TownEventBus.Instance.DispatchOnActionTaken(this.actionTaken, this.maxAction);
        }
    }
}