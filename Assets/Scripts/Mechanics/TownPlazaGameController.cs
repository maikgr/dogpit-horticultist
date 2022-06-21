namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Horticultist.Scripts.UI;

    public class TownPlazaGameController : MonoBehaviour
    {
        [SerializeField] private int maxAction;
        [SerializeField] private TreeVesselController treeVesselController;
        [SerializeField] private NpcFactory npcFactory;
        [SerializeField] private int visitorPerDayAmount;
        [SerializeField] private string townPlazaSceneName;
        [SerializeField] private string assessmentSceneName;
        [SerializeField] private FadeUIController fadeUIController;
        public static TownPlazaGameController Instance { get; private set; }
        private int actionTaken;
        private GameStateController gameState;

        private IDictionary<int, IEnumerable<string>> weekObjectives = new Dictionary<int, IEnumerable<string>>
        {
            { 0, new List<string> {
                "Have 10 cult members or more by the end of the week"
            }},
            { 1, new List<string> {
                "Have 20 cult members or more by the end of the week",
                "Have at least 5 cultist members with cult rank 'High Tomatholyte'"
            }},
            { 2, new List<string> {
                "Grow Tomathotep's vessel"
            }},
            { 3, new List<string> {
                "Offer 5 sacrifices to Tomathotep's vessel"
            }},
        };

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<TownPlazaGameController>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TownPlazaGameController can be active!");
            }

            Instance = this;
        }

        private void OnEnable() {
            StartCoroutine(OnEnableCoroutine());
        }

        private IEnumerator OnEnableCoroutine()
        {
            while(TownEventBus.Instance == null)
            {
                yield return new WaitForFixedUpdate();
            }
            TownEventBus.Instance.OnCultistJoin += OnCultistJoin;
            TownEventBus.Instance.OnCultistLeave += OnCultistLeave;
            
            SceneManager.activeSceneChanged += ChangedActiveScene;
        }

        private void OnDisable() {
            TownEventBus.Instance.OnCultistJoin -= OnCultistJoin;
            TownEventBus.Instance.OnCultistLeave -= OnCultistLeave;

            SceneManager.activeSceneChanged -= ChangedActiveScene;
        }

        private void Start() {
            gameState = GameStateController.Instance;
            StartCoroutine(DelayedStart());
        }

        // Make sure all listeners are ready;
        private IEnumerator DelayedStart()
        {
            GenerateVisitors();

            yield return new WaitForSeconds(0.2f);
            TownEventBus.Instance.DispatchOnDayStart(gameState.WeekNumber, gameState.DayNumber);
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[0]);
        }

        private void ChangedActiveScene(Scene prev, Scene next)
        {
            if (prev.name == assessmentSceneName)
            {
                TownEventBus.Instance.DispatchOnDayStart(gameState.WeekNumber, gameState.DayNumber);
            }
        }

        public void EndDay()
        {
            this.actionTaken = 0;
            TownEventBus.Instance.DispatchOnActionTaken(this.actionTaken, this.maxAction);
            GoNextDay();
        }

        private void GoNextDay()
        {
            TownEventBus.Instance.DispatchOnDayEnd(gameState.WeekNumber, gameState.DayNumber);
            
            fadeUIController.FadeOutScreen(() => {
                SceneManager.LoadScene(assessmentSceneName);
            });
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

        private void OnCultistJoin(NpcController npc)
        {
            gameState.CultMembers.Add(npc);
        }

        private void OnCultistLeave(NpcController npc)
        {
            gameState.CultMembers.Remove(npc);
        }

        private void GenerateVisitors()
        {
            for (var i = 0; i < visitorPerDayAmount; ++i)
            {
                npcFactory.GenerateNpc();
            }
        }
    }
}