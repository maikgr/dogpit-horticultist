namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TownPlazaGameController : MonoBehaviour
    {
        [Header("Action Properties")]
        [SerializeField] private int maxAction;
        public static TownPlazaGameController Instance { get; private set; }
        private int weekNumber;
        private int dayNumber;
        private int actionTaken;
        public List<NpcController> CultMembers { get; private set; }

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
            this.weekNumber = 1;
            this.dayNumber = 1;
            TownEventBus.Instance.DispatchOnDayStart(this.weekNumber, this.dayNumber);
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
    }
}