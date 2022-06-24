namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.UI;
    using Horticultist.Scripts.Core;

    public class TownPlazaGameController : MonoBehaviour
    {
        [SerializeField] private NpcFactory npcFactory;
        [SerializeField] private int visitorPerDayAmount;
        public static TownPlazaGameController Instance { get; private set; }
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

        private void Start() {
            gameState = GameStateController.Instance;
            StartCoroutine(DelayedStart());
        }

        // Make sure all listeners are ready;
        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(0.2f);
            if (this.gameState.PrevScene == SceneNameConstant.ASSESSMENT || this.gameState.ActionTaken == 0)
            {
                GenerateVisitors();
            }

            TownEventBus.Instance.DispatchOnDayStart(gameState.WeekNumber, gameState.DayNumber);
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[0]);
            GameStateController.Instance.PrevScene = SceneNameConstant.TOWN_PLAZA;
        }

        public void EndDay()
        {
            this.gameState.ActionTaken = 0;
            TownEventBus.Instance.DispatchOnActionTaken(this.gameState.ActionTaken, this.gameState.MaxAction);
            GoNextDay();
        }

        private void GoNextDay()
        {
            TownEventBus.Instance.DispatchOnDayEnd(gameState.WeekNumber, gameState.DayNumber);
        }

        public void AddAction()
        {
            this.gameState.ActionTaken += 1;
            if (this.gameState.ActionTaken >= this.gameState.MaxAction)
            {
                this.gameState.ActionTaken = 0;
                GoNextDay();
            }
            TownEventBus.Instance.DispatchOnActionTaken(this.gameState.ActionTaken, this.gameState.MaxAction);
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