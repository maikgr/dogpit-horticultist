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
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private SpriteRenderer headgearSprite;
        [SerializeField] private SpriteRenderer eyesSprite;
        [SerializeField] private SpriteRenderer mouthSprite;

        [Header("Walkable Area")]
        [SerializeField] private Vector2 minAreaPoint;
        [SerializeField] private Vector2 maxAreaPoint;
        private string firstName;
        private string lastName;
        public NpcTypeEnum NpcType { get; private set; }
        public int IndoctrinationValue { get; private set; }
        public string DisplayName => $"{firstName} {lastName}";

        private void Start() {
            StartCoroutine(WalkAround());
        }

        public void GenerateVisitor(string firstName, string lastName,
            Sprite bodyAsset, Sprite headgearAsset)
        {
            this.firstName = firstName;
            this.lastName = lastName;
            npcName.text = firstName + " " + lastName;
            bodySprite.sprite = bodyAsset;
            headgearSprite.sprite = headgearAsset;
            NpcType = NpcTypeEnum.Visitor;
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

            var destination = GetRandomDestination();
            var tween = transform.DOMove(destination, 0.5f)
                .SetSpeedBased(true)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    StopCoroutine(WalkAround());
                    StartCoroutine(WalkAround());
                })
                .SetId("npc");
        }

        
        private Vector2 GetRandomDestination()
        {
            var posX = Random.Range(minAreaPoint.x, maxAreaPoint.x);
            var posY = Random.Range(minAreaPoint.y, maxAreaPoint.y);

            return new Vector2(posX, posY);
        }
    }
}
