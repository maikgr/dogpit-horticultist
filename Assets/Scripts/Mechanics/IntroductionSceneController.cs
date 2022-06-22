namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using TMPro;
    using Horticultist.Scripts.UI;

    public class IntroductionSceneController : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private FadeUIController fadeUIController;
        [SerializeField] private string nextSceneName;
        private HorticultistInputActions gameInputs;
        private bool isTyping;
        private int currentIndex;

        Coroutine c;
        bool coroutineHasStartedAtLeastOnce = false;

        private void Awake()
        {
            gameInputs = new HorticultistInputActions();
        }

        private void OnEnable()
        {
            gameInputs.UI.Click.performed += OnClickPerformed;
            gameInputs.UI.Click.Enable();
        }

        private void OnDisable()
        {
            gameInputs.UI.Click.performed -= OnClickPerformed;
            gameInputs.UI.Click.Disable();
        }

        private void Start()
        {
            fadeUIController.FadeInScreen(
                () => { 
                    c = StartCoroutine(ShowDialogue(currentIndex)); 
                    coroutineHasStartedAtLeastOnce = true;
                }
            );
        }

        private List<DialogueSceneText> dialogues = new List<DialogueSceneText>
        {
            new DialogueSceneText {
                name = "Tomathotep",
                text = "I am Tomathotep, god of death and destruction! And you shall build a vessel for me!"
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "There is no mercy for those who cross me. Now get to work!"
            },
        };

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (!coroutineHasStartedAtLeastOnce) return;
            if (context.ReadValue<float>() == 0) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    c = StartCoroutine(ShowDialogue(nextIndex));
                    currentIndex = nextIndex;
                }
                else
                {
                    fadeUIController.FadeOutScreen(
                        () => SceneManager.LoadScene(nextSceneName)
                    );
                }
            }
            else 
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = dialogues[currentIndex].text;
            }
        }

        private IEnumerator ShowDialogue(int index)
        {
            isTyping = true;
            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
            dialogueText.text = string.Empty;
            while (curLetterIdx < text.Length)
            {

                dialogueText.text += text[curLetterIdx++];
                yield return new WaitForFixedUpdate();
            }
            isTyping = false;
        }
    }
}