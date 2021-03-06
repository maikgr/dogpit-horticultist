namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    using Random = UnityEngine.Random;
    using TMPro;
    using UnityEngine.UI;
    using DG.Tweening;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.UI;
    using Pathfinding;

    public class NpcController : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        public Sprite bodySprite => bodySpriteRenderer.sprite;
        [SerializeField] private Animator bodyAnimator;
        [SerializeField] private SpriteRenderer headgearSpriteRenderer;
        public Sprite headgearSprite => headgearSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer eyesSpriteRenderer;
        public Sprite eyesSprite => eyesSpriteRenderer.sprite;
        [SerializeField] private SpriteRenderer mouthSpriteRenderer;
        public Sprite mouthSprite => mouthSpriteRenderer.sprite;
        [SerializeField] private Sprite cultistHat;
        private NpcExpressionSet eyesExpressionSet;
        private NpcExpressionSet mouthExpressionSet;
        [SerializeField] private Animator sacrificeAnimator;
        [SerializeField] private Color highlightColor;
        public bool IsHighlighted { get; private set; }
        public bool IsHovered { get; private set; }

        [Header("Mechanics")]
        [SerializeField] private int obedienceChangeValue;
        [SerializeField] private int obedienceDecayValue;
        [SerializeField] private int sacrificeValue;
        public string NpcID { get; private set; }
        public SpecialNpcTypeEnum? SpecialNpcType { get; private set; }

        [Header("UI")]
        [SerializeField] private List<TypeColourMap> typeColourMaps;
        [SerializeField] private Image npcTypeCanvas;
        public Color canvasColor => npcTypeCanvas.color;


        // NPC Type Properties
        public string DisplayName { get; private set; }
        public NpcTypeEnum NpcType { get; private set; }
        public NpcPersonalityEnum npcPersonality { get; private set; }
        public NpcDialogueSet DialogueSet { get; private set; }
        public int PatienceValue { get; private set; }
        public int IndoctrinationValue { get; private set; }

        // Townspeople Props
        public MoodEnum moodType { get; private set; }

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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            sacrificeAnimator.enabled = false;
        }

        private void Start()
        {
            GameStateController.Instance.AddNpc(this);
        }

        private void OnEnable()
        {
            TownEventBus.Instance.OnDayEnd += OnDayEnd;
        }

        private void OnDisable()
        {
            TownEventBus.Instance.OnDayEnd -= OnDayEnd;
            DOTween.Kill(NpcID);
        }

        private void FixedUpdate()
        {
            if (sacrificeAnimator.enabled && sacrificeAnimator.GetCurrentAnimatorStateInfo(0).IsName("sacrifice_end"))
            {
                OnSacrificeAnimationEnds();
            }

            if (AbsorbAnimator.gameObject.activeSelf && AbsorbAnimator.GetCurrentAnimatorStateInfo(0).IsName("vine_absorbs_end"))
            {
                if (onAbsorbEnds != null)
                {
                    onAbsorbEnds.Invoke();
                }
                AbsorbAnimator.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            GameStateController.Instance.RemoveNpc(this);
        }

        public void ConfigureGeneric(string firstName, string lastName, NpcPersonalityEnum personality,
            NpcDialogueSet dialogueSet, List<CultistObedienceAction> obedienceActions,
            NpcBodySet bodySet, Sprite headgearAsset, NpcExpressionSet eyesSet, NpcExpressionSet mouthSet)
        {
            // Basic Info
            DisplayName = firstName + " " + lastName;
            npcNameText.text = firstName;
            npcTypeText.text = NpcTypeEnum.Visitor.ToString();

            // Visual Assets
            bodySpriteRenderer.sprite = bodySet.body;
            headgearSpriteRenderer.sprite = headgearAsset;
            eyesExpressionSet = eyesSet;
            mouthExpressionSet = mouthSet;
            eyesSpriteRenderer.sprite = eyesSet.neutral;
            mouthSpriteRenderer.sprite = mouthSet.neutral;
            bodyAnimator.runtimeAnimatorController = bodySet.animatorController;

            // Mechanic props
            NpcType = NpcTypeEnum.Visitor;
            npcPersonality = personality;
            PatienceValue = 100;
            IndoctrinationValue = 0;
            NpcID = Guid.NewGuid().ToString();

            // Dialogues
            DialogueSet = dialogueSet;
            this.obedienceActions = obedienceActions;
        }

        public void ConfigureSpecial(SpecialNpcTypeEnum specialNpcType, NpcPersonalityEnum personality,
            NpcBodySet bodySet, NpcDialogueSet dialogueSet, List<CultistObedienceAction> obedienceActions,
            NpcExpressionSet expressionSet)
        {
            // Basic Info
            SpecialNpcType = specialNpcType;
            DisplayName = npcNameText.text;
            npcTypeText.text = NpcTypeEnum.Visitor.ToString();

            // Visual Assets
            bodySpriteRenderer.sprite = bodySet.body;
            eyesExpressionSet = expressionSet;
            eyesSpriteRenderer.sprite = expressionSet.neutral;
            bodyAnimator.runtimeAnimatorController = bodySet.animatorController;

            // Mechanic props
            NpcType = NpcTypeEnum.Visitor;
            npcPersonality = personality;
            PatienceValue = 100;
            IndoctrinationValue = 0;
            NpcID = Guid.NewGuid().ToString();

            // Dialogues
            DialogueSet = dialogueSet;
            this.obedienceActions = obedienceActions;
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
            var colour = typeColourMaps.First(t => t.npcType == npcTypeEnum).colour;
            npcTypeCanvas.color = colour;

            this.NpcType = npcTypeEnum;
            npcTypeText.text = npcTypeEnum.ToString();

            if (npcTypeEnum.Equals(NpcTypeEnum.Cultist))
            {
                this.CultistRank = CultistRankEnum.Rank1;
                this.headgearSpriteRenderer.sprite = cultistHat;
                ObedienceValue = 0;
                HasObedienceAction = false;
                ObedienceDialogue = DialogueSet.therapy.success.GetRandom();
                npcTypeText.text = this.CultistRank.DisplayString();
            }
            if (SpecialNpcType.HasValue)
            {
                this.headgearSpriteRenderer.gameObject.SetActive(true);
            }
        }

        public void SetMood(MoodEnum moodType)
        {
            this.moodType = moodType;
            if (moodType == MoodEnum.Happy)
            {
                this.eyesSpriteRenderer.sprite = this.eyesExpressionSet.happy;
                if (!SpecialNpcType.HasValue)
                {
                    this.mouthSpriteRenderer.sprite = this.mouthExpressionSet.happy;
                }
            }
            else if (moodType == MoodEnum.Angry)
            {
                this.eyesSpriteRenderer.sprite = this.eyesExpressionSet.happy;
                if (!SpecialNpcType.HasValue)
                {
                    this.mouthSpriteRenderer.sprite = this.mouthExpressionSet.happy;
                }
            }
            else
            {
                this.eyesSpriteRenderer.sprite = this.eyesExpressionSet.neutral;
                if (!SpecialNpcType.HasValue)
                {
                    this.mouthSpriteRenderer.sprite = this.mouthExpressionSet.neutral;
                }
            }
        }

        public void DecreaseObedienceValue(int val)
        {
            this.ObedienceValue = Mathf.Max(this.ObedienceValue - val, -6);
            TownEventBus.Instance.DispatchOnObedienceLevelChange(this.ObedienceLevel);
        }
        public void IncreaseObedienceValue(int val)
        {
            this.ObedienceValue = Mathf.Min(this.ObedienceValue + val, 6);
            TownEventBus.Instance.DispatchOnObedienceLevelChange(this.ObedienceLevel);
        }

        public void AnswerObedienceAction(CultistObedienceActionEnum action)
        {
            if (!HasObedienceAction) return;
            if (currentObedienceAction.action.Equals(action))
            {
                IncreaseObedienceValue(4);
                ObedienceDialogue = DialogueSet.therapy.moodup.GetRandom();
                SetMood(MoodEnum.Happy);

                if (action == CultistObedienceActionEnum.Praise)
                {
                    SfxController.Instance.PlaySfx(SfxEnum.PraiseRight);
                }
                else 
                {
                    SfxController.Instance.PlaySfx(SfxEnum.ScoldRight);
                }
            }
            else
            {
                DecreaseObedienceValue(4);
                ObedienceDialogue = DialogueSet.therapy.mooddown.GetRandom();
                SetMood(MoodEnum.Angry);

                if (action == CultistObedienceActionEnum.Praise)
                {
                    SfxController.Instance.PlaySfx(SfxEnum.PraiseWrong);
                }
                else 
                {
                    SfxController.Instance.PlaySfx(SfxEnum.ScoldWrong);
                }
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
            DOTween.Kill(NpcID);
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
                .SetId(NpcID)
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
                npcTypeText.text = this.CultistRank.DisplayString();
            }
            else if (HasObedienceAction)
            {
                DecreaseObedienceValue(2);
            }

            if (NpcType.Equals(NpcTypeEnum.Cultist))
            {
                currentObedienceAction = obedienceActions.GetRandom();
                ObedienceDialogue = currentObedienceAction.text;
                HasObedienceAction = true;
            }
        }

        private Action onSacrificeEnds;
        public void Sacrifice(Action onSacrificeEnds = null)
        {
            this.onSacrificeEnds = onSacrificeEnds;
            sacrificeAnimator.enabled = true;
        }

        private void OnSacrificeAnimationEnds()
        {
            sacrificeAnimator.enabled = false;
            if (onSacrificeEnds != null)
            {
                onSacrificeEnds.Invoke();
            }
            var newHeight = GameStateController.Instance.TreeHeight + sacrificeValue;
            GameStateController.Instance.SetTreeStatus(
                GameStateController.Instance.TreeStage,
                newHeight
            );
            GameStateController.Instance.SacrificedMembers.Add(DisplayName);
            Destroy(gameObject);
        }

        private Action onAbsorbEnds;
        [SerializeField] private Animator AbsorbAnimator;
        public void AbsorbPointsAnimation(Action onAbsorbEnds = null)
        {
            this.onAbsorbEnds = onAbsorbEnds;
            AbsorbAnimator.gameObject.SetActive(true);
        }
    }

    [Serializable]
    public class NpcExpressionSet
    {
        public Sprite neutral;
        public Sprite happy;
        public Sprite angry;
    }

    [Serializable]
    public class NpcBodySet
    {
        public Sprite body;
        public RuntimeAnimatorController animatorController;
    }

    [Serializable]
    public class TypeColourMap
    {
        public NpcTypeEnum npcType;
        public Color colour;
    }
}
