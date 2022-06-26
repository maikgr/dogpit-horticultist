namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Extensions;
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
                "Have 6 cult members or more"
            }},
            { 1, new List<string> {
                "Have 13 cult members or more",
                "Have at least 1 cultist members with cult rank 'Horticultist'"
            }},
            { 2, new List<string> {
                "Grow Tomathotep's vessel"
            }},
            { 3, new List<string> {
                "Offer a sacrifice to Tomathotep's vessel"
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
            if (this.gameState.PrevScene == SceneNameConstant.ASSESSMENT ||
                this.gameState.PrevScene == SceneNameConstant.TREE_GROWTH ||
                this.gameState.ActionTaken == 0)
            {
                var numberToGenerate = visitorPerDayAmount;
                if (this.gameState.WeekNumber > 1)
                {
                    var isSpecial = Random.Range(0, 1f) < 0.5f ||
                        (
                            gameState.SpecialSpawnedThisWeek.Count == 0 &&
                            gameState.SpawnableSpecialNpcs.Count > 0 &&
                            gameState.DayNumber == gameState.DaysPerAssessment
                        );

                    if (isSpecial)
                    {
                        numberToGenerate -= 1;
                        
                        var specialToGenerate = gameState.SpawnableSpecialNpcs.GetRandom();
                        Debug.Log("spawning special " + specialToGenerate.ToString());
                        gameState.SpawnableSpecialNpcs.Remove(specialToGenerate);
                        gameState.SpecialSpawnedThisWeek.Add(specialToGenerate);

                        var specialNpc = npcFactory.SpawnSpecialNpc(specialToGenerate);
                    }
                }
                GenerateVisitors(numberToGenerate);
            }

            TownEventBus.Instance.DispatchOnDayStart(gameState.WeekNumber, gameState.DayNumber);
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[gameState.WeekNumber]);
            gameState.PrevScene = SceneNameConstant.TOWN_PLAZA;
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

        private void GenerateVisitors(int amount)
        {
            for (var i = 0; i < amount; ++i)
            {
                npcFactory.SpawnGenericNpc();
            }
        }
    }
}