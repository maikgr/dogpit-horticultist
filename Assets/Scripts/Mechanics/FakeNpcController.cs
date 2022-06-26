namespace Horticultist.Scripts.Mechanics
{
    using System;
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;

    public class FakeNpcController : MonoBehaviour
    {
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private SpriteRenderer bodySpriteRenderer;
        [SerializeField] private SpriteRenderer headgearSpriteRenderer;
        [SerializeField] private SpriteRenderer eyesSpriteRenderer;
        [SerializeField] private SpriteRenderer mouthSpriteRenderer;
        [SerializeField] private Image npcTypeCanvas;

        private Action onAbsorbEnds;
        [SerializeField] private Animator AbsorbAnimator;

        private void FixedUpdate()
        {
            if (AbsorbAnimator.gameObject.activeSelf && AbsorbAnimator.GetCurrentAnimatorStateInfo(0).IsName("vine_absorbs_end"))
            {
                if (onAbsorbEnds != null)
                {
                    onAbsorbEnds.Invoke();
                }
                AbsorbAnimator.gameObject.SetActive(false);
            }
        }

        public void Configure(NpcController source)
        {
            Debug.Log("Configuring NPC " + source.DisplayName);
            // Basic Info
            npcNameText.text = source.DisplayName;
            npcTypeText.text = source.NpcType.ToString();

            // Visual Assets
            SetSprite(bodySpriteRenderer, source.bodySprite);
            SetSprite(headgearSpriteRenderer, source.headgearSprite);
            SetSprite(eyesSpriteRenderer, source.eyesSprite);
            SetSprite(mouthSpriteRenderer, source.mouthSprite);
            npcTypeCanvas.color = source.canvasColor;
            
            Debug.Log("Done configure NPC");
        }

        private void SetSprite(SpriteRenderer renderer, Sprite sourceSprite)
        {
            renderer.sprite = sourceSprite;
            renderer.enabled = sourceSprite != null;
        }

        public void StartAbsorbAnimation(Action onAbsorbEnds = null)
        {
            this.onAbsorbEnds = onAbsorbEnds;
            AbsorbAnimator.gameObject.SetActive(true);
        }
    }
}
