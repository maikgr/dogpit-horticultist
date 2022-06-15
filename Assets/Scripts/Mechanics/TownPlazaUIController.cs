namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Horticultist.Scripts.Core;

    public class TownPlazaUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text weekDayText;
        [SerializeField] private TMP_Text actionText;

        private void OnEnable() {
            TownEventBus.Instance.OnDayStart += UpdateWeekDayUI;
            TownEventBus.Instance.OnActionTaken += UpdateActionUI;
        }

        private void UpdateWeekDayUI(int weekNumber, int dayNumber)
        {
            weekDayText.text = $"Week {weekNumber} Day {dayNumber}";
        }

        private void UpdateActionUI(int taken, int max)
        {
            actionText.text = $"Action {taken}/{max}";
        }
    }
}