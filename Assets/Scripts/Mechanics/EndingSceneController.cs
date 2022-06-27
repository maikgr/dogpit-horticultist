namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using TMPro;
    using DG.Tweening;
    using Horticultist.Scripts.Core;

    public class EndingSceneController : MonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image CgImage;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private List<EndingSceneCG> endingSceneCGs;
        private HorticultistInputActions gameInputs;
        private bool isTyping;
        private int currentIndex;
        private List<string> endTexts;

        private void Awake()
        {
            gameInputs = new HorticultistInputActions();
        }

        private void OnEnable() {
            gameInputs.UI.Click.performed += OnClickPerformed;
            gameInputs.UI.Click.Enable();
        }

        private void OnDisable() {
            gameInputs.UI.Click.performed -= OnClickPerformed;
            gameInputs.UI.Click.Disable();
        }

        private void Start() {
            var ending = endingSceneCGs.First(e => e.endingType == GameStateController.Instance.EndingType);
            CgImage.sprite = ending.endingCg;
            bgmSource.clip = ending.audioBgm;
            bgmSource.Play();
            this.endTexts = ending.endTexts;
            dialogueText.text = endTexts[currentIndex];
        }


        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() == 0) return;
            var nextIndex = currentIndex + 1;
            if (nextIndex < endTexts.Count)
            {
                dialogueText.text += "\n\n" + endTexts[nextIndex];
                currentIndex = nextIndex;
            }
        }
    }

    [System.Serializable]
    public class EndingSceneCG
    {
        public EndingTypeEnum endingType;
        public Sprite endingCg;
        public AudioClip audioBgm;
        [TextArea]
        public List<string> endTexts;
    }
}