namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Horticultist.Scripts.Core;
    using DG.Tweening;

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
        public NpcTypeEnum NpcType { get; private set; }
        public int PatienceValue { get; private set; }
        public int IndoctrinationValue { get; private set; }
        public int ObedienceValue { get; private set; }
        public string ObedienceLevel { get; private set; }
        public string DisplayName { get; private set; }

        private void Start() {
            StartCoroutine(WalkAround());
        }

        public void GenerateVisitor(string name,
            Sprite bodyAsset, Sprite headgearAsset, Sprite eyesAsset, Sprite mouthAsset)
        {
            DisplayName = name;
            npcName.text = name;
            bodySpriteRenderer.sprite = bodyAsset;
            headgearSpriteRenderer.sprite = headgearAsset;
            eyesSpriteRenderer.sprite = eyesAsset;
            mouthSpriteRenderer.sprite = mouthAsset;
            NpcType = NpcTypeEnum.Visitor;
            PatienceValue = 100;
            IndoctrinationValue = 0;
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
    }
}
