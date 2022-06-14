namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Horticultist.Scripts.Core;

    public class NpcController : MonoBehaviour
    {
        [Header("Assets")]
        [SerializeField] private TMP_Text npcName;
        [SerializeField] private SpriteRenderer bodySprite;
        [SerializeField] private SpriteRenderer headgearSprite;
        [SerializeField] private SpriteRenderer eyesSprite;
        [SerializeField] private SpriteRenderer mouthSprite;
        private string firstName;
        private string lastName;
        public NpcTypeEnum NpcType { get; private set; }
        public int IndoctrinationValue { get; private set; }
        public string DisplayName => $"{firstName} {lastName}";

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
    }
}
