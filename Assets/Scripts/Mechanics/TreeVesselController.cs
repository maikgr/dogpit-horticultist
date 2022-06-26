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
        
        private void Start() {
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
        }
    }

    [Serializable]
    public struct TreeVesselStage
    {
        public float Threshold;
        public int Stage;
        public GameObject TreeGameObject;
    }
}
