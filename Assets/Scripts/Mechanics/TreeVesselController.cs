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
            UpdateGrowth(0);
        }

        private void OnDayEnd(int week, int day)
        {
            var totalEf = GameStateController.Instance.CultMembers.Sum(m => m.EfficiencyValue);
            UpdateGrowth(totalEf);
        }

        private void UpdateGrowth(int value)
        {
            // Disable all tree sprites
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));

            // Calculate tree growth
            var height = GameStateController.Instance.TreeHeight + value;
            var treeStage = stageValueThreshold.Where(val => val.Threshold <= height)
                .OrderByDescending(val => val.Threshold)
                .First();

            treeStage.TreeGameObject.SetActive(true);
            GameStateController.Instance.SetTreeStatus(treeStage.Stage, height);
            TownEventBus.Instance.DispatchOnTreeGrowthChange(height, treeStage.Stage);
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
