namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TownPlazaGameController : MonoBehaviour
    {
        [SerializeField] private int maxAction;
        [SerializeField] private TreeVesselController treeVesselController;
        public static TownPlazaGameController Instance { get; private set; }
        public List<NpcController> CultMembers { get; private set; }
        public List<string> SacrificedMembers { get; set; }
        private int weekNumber;
        private int dayNumber;
        private int actionTaken;

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
                "Offer 10 sacrifices to Tomathotep's vessel"
            }},
        };

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<TownPlazaGameController>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one TownPlazaGameController can be active!");
            }

            Instance = this;

            CultMembers = new List<NpcController>();
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
        }

        private void OnDisable() {
            TownEventBus.Instance.OnCultistJoin -= OnCultistJoin;
            TownEventBus.Instance.OnCultistLeave -= OnCultistLeave;
        }

        private void Start() {
            this.weekNumber = 0;
            this.dayNumber = 1;
            StartCoroutine(DelayedEventDispatch());
        }

        // Make sure all listeners are ready;
        private IEnumerator DelayedEventDispatch()
        {
            yield return new WaitForSeconds(0.2f);
            TownEventBus.Instance.DispatchOnDayStart(this.weekNumber, this.dayNumber);
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[0]);
        }

        public void EndDay()
        {
            this.actionTaken = 0;
            TownEventBus.Instance.DispatchOnActionTaken(this.actionTaken, this.maxAction);
            GoNextDay();
        }

        private void GoNextDay()
        {
            TownEventBus.Instance.DispatchOnDayEnd(this.weekNumber, this.dayNumber);
            this.dayNumber += 1;
            if(this.dayNumber > 7)
            {
                this.weekNumber += 1;
                this.dayNumber = 1;

                switch(this.weekNumber)
                {
                    case 1:
                        Week1Assesment();
                        break;
                    case 2:
                        Week2Assesment();
                        break;
                    case 3:
                        Week3Assesment();
                        break;
                    case 4:
                    default:
                        Week4Assesment();
                        break;
                }
            }
            TownEventBus.Instance.DispatchOnDayStart(this.weekNumber, this.dayNumber);
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
            CultMembers.Add(npc);
        }

        private void OnCultistLeave(NpcController npc)
        {
            CultMembers.Remove(npc);
        }

        private void Week1Assesment()
        {
            if (CultMembers.Count < 10)
            {
                // Warn scene
            }
            else
            {
                // Praise scene
            }
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[1]);
        }

        private void Week2Assesment()
        {
            var rank3Count = CultMembers.Where(mem => mem.CultistRank == Core.CultistRankEnum.Rank3).Count();
            if (CultMembers.Count < 20 || rank3Count < 5)
            {
                // Failed leader ending
            }
            else
            {
                // Praise scene
            }
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[2]);
        }
        private void Week3Assesment()
        {
            if (treeVesselController.CurrentStage > 2)
            {
                // Warn scene
            }
            else
            {
                // Praise scene
            }
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[3]);
        }

        private void Week4Assesment()
        {
            var minRequired = CultMembers.Count * 0.1f;
            var recRequired = CultMembers.Count * 0.3f;
            if (SacrificedMembers.Count < minRequired)
            {
                // Pacifist scene
            }
            else
            {
                // Ultimate cult scene
            }
            TownEventBus.Instance.DispatchOnObjectiveUpdate(weekObjectives[3]);
        }
    }
}