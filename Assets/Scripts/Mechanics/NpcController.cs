namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using DG.Tweening;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.Extensions;

    public class NpcController : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private TMP_Text npcName;
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        public Sprite bodySprite => bodySpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer headgearSpriteRenderer;
        public Sprite headgearSprite => headgearSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer eyesSpriteRenderer;
        public Sprite eyesSprite => eyesSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer mouthSpriteRenderer;
        public Sprite mouthSprite => mouthSpriteRenderer.sprite;
        
        // NPC Type Properties
        public string DisplayName { get; private set; }
        public NpcTypeEnum NpcType { get; private set; }
        public NpcPersonalityEnum npcPersonality { get; private set; }
        public NpcDialogueSet DialogueSet { get; private set; }
        public int PatienceValue { get; private set; }
        public int IndoctrinationValue { get; private set; }

        // Townspeople Props
        public MoodTypeEnum moodType { get; private set; }

        // Cultist Props
        public CultistRankEnum CultistRank { get; private set; }
        private List<CultistObedienceAction> obedienceActions;
        private CultistObedienceAction currentObedienceAction;
        public bool HasObedienceAction { get; private set; }
        public string ObedienceDialogue { get; private set; }
        public int ObedienceValue { get; private set; }
        public int EfficiencyValue => (int)CultistRank + ObedienceValue;
        public CultistObedienceLevelEnum ObedienceLevel => ObdLevelThreshold[ObedienceValue];
        public IDictionary<int, CultistObedienceLevelEnum> ObdLevelThreshold = new Dictionary<int, CultistObedienceLevelEnum>
        {
            {-6, CultistObedienceLevelEnum.VeryRebellious},
            {-5, CultistObedienceLevelEnum.Rebellious},
            {-4, CultistObedienceLevelEnum.Rebellious},
            {-3, CultistObedienceLevelEnum.Rebellious},
            {-2, CultistObedienceLevelEnum.Neutral},
            {-1, CultistObedienceLevelEnum.Neutral},
            {0, CultistObedienceLevelEnum.Neutral},
            {1, CultistObedienceLevelEnum.Neutral},
            {2, CultistObedienceLevelEnum.Neutral},
            {3, CultistObedienceLevelEnum.Obedient},
            {4, CultistObedienceLevelEnum.Obedient},
            {5, CultistObedienceLevelEnum.Obedient},
            {6, CultistObedienceLevelEnum.VeryObedient},
        };

        private void Start() {
            StartCoroutine(WalkAround());
        }

        private void OnEnable() {
            TownEventBus.Instance.OnDayEnd += OnDayEnd;
            TownEventBus.Instance.OnDayStart += OnDayStart;
        }

        private void OnDisable() {
            TownEventBus.Instance.OnDayEnd -= OnDayEnd;
            TownEventBus.Instance.OnDayStart -= OnDayStart;
        }

        private void OnDestroy() {
            DOTween.Kill(transform);
        }

        public void GenerateNpc(string name, NpcPersonalityEnum personality,
            NpcDialogueSet dialogueSet, List<CultistObedienceAction> obedienceActions,
            Sprite bodyAsset, Sprite headgearAsset, Sprite eyesAsset, Sprite mouthAsset)
        {
            // Basic Info
            DisplayName = name;
            npcName.text = name;

            // Visual Assets
            bodySpriteRenderer.sprite = bodyAsset;
            headgearSpriteRenderer.sprite = headgearAsset;
            eyesSpriteRenderer.sprite = eyesAsset;
            mouthSpriteRenderer.sprite = mouthAsset;

            // Mechanic props
            NpcType = NpcTypeEnum.Visitor;
            npcPersonality = personality;
            PatienceValue = 100;
            IndoctrinationValue = 0;

            // Dialogues
            DialogueSet = dialogueSet;
            this.obedienceActions = obedienceActions;
        }

        private IEnumerator WalkAround()
        {
            var isIdle = Random.Range(0f, 1f) < 0.5f;
            while(isIdle)
            {
                yield return new WaitForSeconds(1f);
                isIdle = Random.Range(0f, 1f) < 0.5f;
            }

            var destination = TownPlazaAreaController.Instance.GetRandomPoint();
            var tween = transform.DOMove(destination, 0.5f)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    StopCoroutine(WalkAround());
                    StartCoroutine(WalkAround());
                })
                .SetId("npc");
        }

        public void SetPatience(int value)
        {
            this.PatienceValue = value;
        }

        public void SetIndoctrination(int value)
        {
            this.IndoctrinationValue = value;
        }

        public void ChangeType(NpcTypeEnum npcTypeEnum)
        {
            this.NpcType = npcTypeEnum;
            if (npcTypeEnum.Equals(NpcTypeEnum.Cultist))
            {
                this.CultistRank = CultistRankEnum.Rank1;
                ObedienceValue = 0;
                ObedienceDialogue = DialogueSet.Therapy.Moodup.GetRandom();
                TownEventBus.Instance.DispatchOnCultistJoin(this);
            }
            TownPlazaGameController.Instance.AddAction();
        }

        public void SetMood(MoodTypeEnum moodType)
        {
            this.moodType = moodType;
        }

        public void SetCultistRank(CultistRankEnum cultistRank)
        {
            this.CultistRank = cultistRank;
        }

        public void DecreaseObedienceValue(int val)
        {
            this.ObedienceValue = Mathf.Max(this.ObedienceValue - val, -6);
            Debug.Log(DisplayName + " obd: " + this.ObedienceValue);
            TownEventBus.Instance.DispatchOnObedienceLevelChange(this.ObedienceLevel);
        }
        public void IncreaseObedienceValue(int val)
        {
            this.ObedienceValue = Mathf.Min(this.ObedienceValue + val, 6);
            Debug.Log(DisplayName + " obd: " + this.ObedienceValue);
            TownEventBus.Instance.DispatchOnObedienceLevelChange(this.ObedienceLevel);
        }

        public void ObedienceAction(CultistObedienceActionEnum action)
        {
            if (!HasObedienceAction) return;
            if (currentObedienceAction.Action.Equals(action))
            {
                IncreaseObedienceValue(2);
                ObedienceDialogue = DialogueSet.Therapy.Moodup.GetRandom();
            }
            else
            {
                DecreaseObedienceValue(2);
                ObedienceDialogue = DialogueSet.Therapy.Mooddown.GetRandom();
            }
            
            HasObedienceAction = false;
            TownPlazaGameController.Instance.AddAction();
        }

        private void OnDayEnd(int week, int day)
        {
            if (ObedienceLevel.Equals(CultistObedienceLevelEnum.VeryRebellious))
            {
                TownEventBus.Instance.DispatchOnCultistLeave(this);
                Destroy(gameObject);
            }
            else if (ObedienceLevel.Equals(CultistObedienceLevelEnum.VeryObedient))
            {
                if (CultistRank.Equals(CultistRankEnum.Rank1))
                {
                    CultistRank = CultistRankEnum.Rank2;
                    ObedienceValue = 0;
                }
                else if (CultistRank.Equals(CultistRankEnum.Rank2))
                {
                    CultistRank = CultistRankEnum.Rank3;
                    ObedienceValue = 0;
                }
            }
            else if (HasObedienceAction)
            {
                DecreaseObedienceValue(1);
            }
        }

        private void OnDayStart(int week, int day)
        {
            if (NpcType.Equals(NpcTypeEnum.Cultist))
            {
                currentObedienceAction = obedienceActions.GetRandom();
                HasObedienceAction = true;
            }
        }
    }
}
