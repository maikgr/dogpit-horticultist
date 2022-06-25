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
    using Horticultist.Scripts.Core;

    public class EndingSceneController : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image CgImage;
        [SerializeField] private List<EndingSceneCG> endingSceneCGs;
        private HorticultistInputActions gameInputs;
        private bool isTyping;
        private int currentIndex;

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
            StartCoroutine(ShowDialogue(currentIndex));
        }

        private List<DialogueSceneText> dialogues = new List<DialogueSceneText>
        {
            new DialogueSceneText {
                name = "Tomathotep",
                text = "Do you have anything at all to tell 'a couple of tourists' then?"
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "If I'm not going to talk lightly, all I can give you is..."
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "You are not worthy to be my subject, begone!"
            },
        };

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() == 0) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    StartCoroutine(ShowDialogue(nextIndex));
                    currentIndex = nextIndex;
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        private IEnumerator ShowDialogue(int index)
        {
            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
            dialogueText.text = string.Empty;
            while(curLetterIdx < text.Length)
            {
                isTyping = true;
                dialogueText.text += text[curLetterIdx++];
                yield return new WaitForFixedUpdate();
            }
            isTyping = false;
        }
    }

    [System.Serializable]
    public class EndingSceneCG
    {
        public EndingTypeEnum endingType;
        public Sprite endingCg;
    }
}