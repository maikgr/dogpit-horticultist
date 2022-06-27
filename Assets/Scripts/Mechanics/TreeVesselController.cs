namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public class TreeVesselController : MonoBehaviour
    {
        [SerializeField] private List<TreeVesselStage> stageValueThreshold;
        private float currHeight;

        private void Update() {
            if (currHeight != GameStateController.Instance.TreeHeight)
            {
                currHeight = GameStateController.Instance.TreeHeight;
                UpdateTree(currHeight);
            }
        }
        
        private void UpdateTree(float height) {
            // Disable all tree sprites
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));

            // Select tree stage
            // Calculate tree growth
            var treeStage = stageValueThreshold.Where(val => val.Threshold <= height)
                .OrderByDescending(val => val.Threshold)
                .First();
            treeStage.TreeGameObject.SetActive(true);
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
