namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using DG.Tweening;
    using Horticultist.Scripts.Core;

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
        public int PatienceValue { get; private set; }
        public int IndoctrinationValue { get; private set; }
        public int ObedienceValue { get; private set; }
        public string ObedienceLevel { get; private set; }
        public string DisplayName { get; private set; }
        
        // NPC Type Properties
        public NpcTypeEnum NpcType { get; private set; }
        public NpcPersonalityEnum npcPersonality { get; private set; }
        public NpcDialogueSet DialogueSet { get; private set; }
        public List<CultistObedienceAction> ObedienceActions { get; private set; }

        // Townspeople Props
        public MoodTypeEnum moodType { get; private set; }

        // Cultist Props
        public CultistRankEnum cultistRank { get; private set; }

        private void Start() {
            StartCoroutine(WalkAround());
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
            ObedienceActions = obedienceActions;
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
        }

        public void SetMood(MoodTypeEnum moodType)
        {
            this.moodType = moodType;
        }

        public void SetCultistRank(CultistRankEnum cultistRank)
        {
            this.cultistRank = cultistRank;
        }
    }
}
