namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;

    public class TreeVesselController : MonoBehaviour
    {
        [SerializeField] private List<TreeVesselStage> stageValueThreshold;
        
        private void Start() {
            // Disable all tree sprites
            stageValueThreshold.ForEach(val => val.TreeGameObject.SetActive(false));

            // Select tree stage
            var treeStage = stageValueThreshold.First(val => val.Stage == GameStateController.Instance.TreeStage);
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
