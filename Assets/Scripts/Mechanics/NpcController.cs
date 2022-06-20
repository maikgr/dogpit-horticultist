namespace Horticultist.Scripts.Mechanics
{
    using System;
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
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private Transform visualParent;
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        public Sprite bodySprite => bodySpriteRenderer.sprite;
        [SerializeField] private Animator bodyAnimator;
        [SerializeField] private SpriteRenderer headgearSpriteRenderer;
        public Sprite headgearSprite => headgearSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer eyesSpriteRenderer;
        public Sprite eyesSprite => eyesSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer mouthSpriteRenderer;
        public Sprite mouthSprite => mouthSpriteRenderer.sprite;
        private NpcExpressionSet eyesExpressionSet;
        private NpcExpressionSet mouthExpressionSet;
        [SerializeField] private Color highlightColor;
        public bool IsHighlighted { get; private set; }
        public bool IsHovered { get; private set; }
        
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

        // Mechanics
        private PolygonCollider2D walkArea;
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

        public void GenerateNpc(string firstName, string lastName, NpcPersonalityEnum personality,
            NpcDialogueSet dialogueSet, List<CultistObedienceAction> obedienceActions,
            Sprite bodyAsset, Sprite headgearAsset, NpcExpressionSet eyesSet, NpcExpressionSet mouthSet,
            PolygonCollider2D walkArea)
        {
            // Basic Info
            DisplayName = firstName + " " + lastName;
            npcNameText.text = firstName;
            npcTypeText.text = NpcTypeEnum.Visitor.ToString();
            this.walkArea = walkArea;

            // Visual Assets
            bodySpriteRenderer.sprite = bodyAsset;
            headgearSpriteRenderer.sprite = headgearAsset;
            eyesExpressionSet = eyesSet;
            mouthExpressionSet = mouthSet;
            eyesSpriteRenderer.sprite = eyesSet.neutral;
            mouthSpriteRenderer.sprite = mouthSet.neutral;

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
            var isIdle = true;
            bodyAnimator.SetBool("isWalking", false);
            do
            {
                yield return new WaitForSeconds(1f);
                isIdle = UnityEngine.Random.Range(0f, 1f) < 0.5f;
            }
            while (isIdle);

            bodyAnimator.SetBool("isWalking", true);
            var destination = walkArea.GetRandomPoint();
            if (transform.position.x - destination.x > 0)
            {
                visualParent.DORotate(new Vector3(0, 180, 0), 0.5f)
                    .SetEase(Ease.Linear)
                    .SetId("npc");
            }
            else
            {
                visualParent.DORotate(new Vector3(0, 0, 0), 0.5f)
                    .SetEase(Ease.Linear)
                    .SetId("npc");
            }

            transform.DOMove(destination, 0.5f)
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
            npcTypeText.text = npcTypeEnum.ToString();
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

        public void SetHighlighted()
        {
            this.IsHighlighted = true;
            SetHighlighted(this.bodySpriteRenderer);
            SetHighlighted(this.headgearSpriteRenderer);
            SetHighlighted(this.eyesSpriteRenderer);
            SetHighlighted(this.mouthSpriteRenderer);
        }

        public void UnsetHighlighted()
        {
            this.IsHighlighted = false;
            DOTween.Kill("highlight" + DisplayName);
            UnsetHighlighted(this.bodySpriteRenderer);
            UnsetHighlighted(this.headgearSpriteRenderer);
            UnsetHighlighted(this.eyesSpriteRenderer);
            UnsetHighlighted(this.mouthSpriteRenderer);
        }

        public void SetHovered()
        {
            this.IsHovered = true;
            SetHovered(this.bodySpriteRenderer);
            SetHovered(this.headgearSpriteRenderer);
            SetHovered(this.eyesSpriteRenderer);
            SetHovered(this.mouthSpriteRenderer);
        }

        public void UnsetHovered()
        {
            this.IsHovered = false;
            UnsetHovered(this.bodySpriteRenderer);
            UnsetHovered(this.headgearSpriteRenderer);
            UnsetHovered(this.eyesSpriteRenderer);
            UnsetHovered(this.mouthSpriteRenderer);
        }

        private void SetHighlighted(SpriteRenderer renderer)
        {
            renderer.color = Color.white;
            renderer.DOColor(highlightColor, 0.3f)
                .SetEase(Ease.Linear)
                .SetId("highlight" + DisplayName)
                .SetLoops(-1, LoopType.Yoyo);
        }
        
        private void UnsetHighlighted(SpriteRenderer renderer)
        {
            renderer.color = Color.white;
        }
        
        private void SetHovered(SpriteRenderer renderer)
        {
            renderer.color = highlightColor;
        }
        
        private void UnsetHovered(SpriteRenderer renderer)
        {
            renderer.color = Color.white;
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

    [Serializable]
    public class NpcExpressionSet
    {
        public Sprite neutral;
        public Sprite happy;
        public Sprite angry;
    }
}
