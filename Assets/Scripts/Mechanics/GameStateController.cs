namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Horticultist.Scripts.Core;

    public class GameStateController : MonoBehaviour
    {
        [SerializeField] private int daysPerAssessment;
        [SerializeField] private int maxActionPerDay;
        public static GameStateController Instance { get; private set; }
        public NpcController SelectedNpc { get; private set; }
        public List<NpcController> ActiveNpcs { get; private set; }
        public List<NpcController> CultMembers => ActiveNpcs.Where(npc => npc.NpcType.Equals(NpcTypeEnum.Cultist)).ToList();
        public List<string> SacrificedMembers { get; set; }
        public int TreeStage { get; private set; }
        public float TreeHeight { get; private set; }
        public int ActionTaken { get; set; }
        public int MaxAction => maxActionPerDay;
        public int DayNumber { get; private set; }
        public int WeekNumber { get; private set; }
        public bool PlayerHasConverted => ConvertCount > 0;
        public int ConvertCount { get; private set; }
        public string PlayerName { get; private set; }
        public string PrevScene { get; set; }

        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<GameStateController>();
            if (controllers.Length > 1)
            {
                Debug.Log("Found existing GameStateController, destroying");
                Destroy(this.gameObject);
            }

            Instance = this;
            GameObject.DontDestroyOnLoad(this.gameObject);

            ActiveNpcs = new List<NpcController>();
            SacrificedMembers = new List<string>();
        }

        private void Start() {
            ConvertCount = 0;
            DayNumber = 1;
            WeekNumber = 0;
            TreeStage = 1;
            TreeHeight = 1;
        }
        private void OnEnable() {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable() {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void OnActiveSceneChanged(Scene prev, Scene next)
        {
            if (next.name != SceneNameConstant.TOWN_PLAZA)
            {
                ActiveNpcs.ForEach(npc => npc.gameObject.SetActive(false));
            }
            else
            {
                ActiveNpcs.ForEach(npc => npc.gameObject.SetActive(true));
            }
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

        public void SetTreeStatus(int stage, float height)
        {
            TreeStage = stage;
            TreeHeight = height;
        }

        public void AddNpc(NpcController npc)
        {
            ActiveNpcs.Add(npc);
        }

        public void RemoveNpc(NpcController npc)
        {
            ActiveNpcs.Remove(npc);
        }
    }
}