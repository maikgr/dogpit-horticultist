namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameStateController : MonoBehaviour
    {
        [SerializeField] private int daysPerAssessment;
        public static GameStateController Instance { get; private set; }
        public NpcController SelectedNpc { get; private set; }
        public List<NpcController> CultMembers { get; private set; }
        public List<string> SacrificedMembers { get; set; }
        public int TreeStage { get; private set; }
        public float TreeHeight { get; private set; }
        public int DayNumber { get; private set; }
        public int WeekNumber { get; private set; }
        public bool PlayerHasConverted => ConvertCount > 0;
        public int ConvertCount { get; private set; }

        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<GameStateController>();
            if (controllers.Length > 1)
            {
                Destroy(this.gameObject);
            }

            Instance = this;
            GameObject.DontDestroyOnLoad(this);

            CultMembers = new List<NpcController>();
            SacrificedMembers = new List<string>();
        }

        private void Start() {
            ConvertCount = 0;
            DayNumber = 1;
            WeekNumber = 0;
        }

        public void SetSelectedNpc(NpcController npc)
        {
            SelectedNpc = npc;
        }

        public void AddConvertCount()
        {
            ConvertCount += 1;
        }

        public void AddDay()
        {
            DayNumber += 1;
            if (DayNumber > daysPerAssessment)
            {
                DayNumber = 1;
                WeekNumber += 1;
            }
        }

        public void SetTreeStatus(int stage, int height)
        {
            TreeStage = stage;
            TreeHeight = height;
        }
    }
}