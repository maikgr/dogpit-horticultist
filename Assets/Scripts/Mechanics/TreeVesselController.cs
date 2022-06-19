namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TreeVesselController : MonoBehaviour
    {
        [SerializeField] private List<TreeVesselStage> stageValueThreshold;
        public int GrowthValue { get; private set; }
        public int CurrentStage { get; private set; }
        private void OnEnable() {
            StartCoroutine(OnEnableCoroutine());
        }

        private IEnumerator OnEnableCoroutine()
        {
            while(TownEventBus.Instance == null)
            {
                yield return new WaitForFixedUpdate();
            }
            TownEventBus.Instance.OnDayEnd += OnDayEnd;
        }

        private void OnDisable() {
            TownEventBus.Instance.OnDayEnd -= OnDayEnd;
        }

        private void Start() {
            GrowthValue = 1;
            UpdateGrowth(0);
        }

        private void OnDayEnd(int week, int day)
        {
            var totalEf = TownPlazaGameController.Instance.CultMembers.Sum(m => m.EfficiencyValue);
            UpdateGrowth(totalEf);
        }

        private void UpdateGrowth(int value)
        {
            GrowthValue += value;
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));
            var treeStage = stageValueThreshold.Where(val => val.Threshold <= GrowthValue)
                .OrderByDescending(val => val.Threshold)
                .First();

            treeStage.TreeGameObject.SetActive(true);
            CurrentStage = treeStage.Stage;
            TownEventBus.Instance.DispatchOnTreeGrowthChange(GrowthValue, treeStage.Stage);
        }
    }

    [Serializable]
    public struct TreeVesselStage
    {
        public int Threshold;
        public int Stage;
        public GameObject TreeGameObject;
    }
}
