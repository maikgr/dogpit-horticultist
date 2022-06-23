namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.Core;

    public class TherapyCanvasController : MonoBehaviour
    {
        [Header("NPC Basic Info UI")]
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcDialogueText;

        [Header("NPC Visual UI")]
        [SerializeField] private Image npcBodyImage;
        [SerializeField] private Image npcHeadgearImage;
        [SerializeField] private Image npcEyesImage;
        [SerializeField] private Image npcMouthImage;

        [Header("NPC Paremeters UI")]
        [SerializeField] private Image patienceBar;
        [SerializeField] private Image indoctrinationbar;

        // Gameplay mechanic props
        private NpcController currentNpc;
        
        private void Start() {
            currentNpc = GameStateController.Instance.SelectedNpc;

            // Basic info
            npcTypeText.text = currentNpc.NpcType.ToString();
            npcNameText.text = currentNpc.DisplayName;
            npcDialogueText.text = currentNpc.DialogueSet.Therapy.Intro.GetRandom();

            // Visuals
            npcBodyImage.sprite = currentNpc.bodySprite;
            npcEyesImage.sprite = currentNpc.eyesSprite;
            npcMouthImage.sprite = currentNpc.mouthSprite;
            if (currentNpc.headgearSprite == null)
            {
                npcHeadgearImage.enabled = false;
            }
            else
            {
                npcHeadgearImage.enabled = true;
                npcHeadgearImage.sprite = currentNpc.headgearSprite;
            }

            indoctrinationbar.fillAmount = currentNpc.IndoctrinationValue / 100f;
            patienceBar.fillAmount = currentNpc.PatienceValue / 100f;
        }
    }
}