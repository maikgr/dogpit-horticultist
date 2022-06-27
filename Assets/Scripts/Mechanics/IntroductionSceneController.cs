namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using TMPro;
    using Horticultist.Scripts.UI;
    using Horticultist.Scripts.Core;

    public class IntroductionSceneController : MonoBehaviour
    {
        [SerializeField] private Transform nameParent;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Transform dialogueParent;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private FadeUIController fadeUIController;
        private bool isTyping;
        private int currentIndex;

        Coroutine c;
        bool coroutineHasStartedAtLeastOnce = false;

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
                text = "Hello, mortal.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "Does your world not have talking vegetables? You should not be surprised to see me.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "I am Tomathotep. The inconceivably great pleasure of meeting you, is all mine.",
                sfxName = SfxEnum.VLIAmTomathotep
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "Now, I am in need of some earthly assistance. And you, My special friend, are perfect for the job. Shall we agree to a contract of… mutual benefit?",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "Fear not, it is a very simple task. Allow me to explain.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "You must help me grow My vessel. Think of it as… ah, building a house for your friend.",
                sfxName = SfxEnum.VlMyVessel
            },  
            new DialogueSceneText {
                name = "Tomathotep",
                text = "However, where your houses are built with sticks and stone, mine is formed from the prayers of the faithful, and the blood of the worthy.",
                sfxName = SfxEnum.None,
            },   
            new DialogueSceneText {
                name = "Tomathotep",
                text = "You must gather more like-minded folk, who will, together with you, grow enlightened in our ways. The ways of Tomathotep.",
                sfxName = SfxEnum.None,
            },   
            new DialogueSceneText {
                name = "Tomathotep",
                text = "We are making a family, where everyone can find love, acceptance and peace.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "Go out into the village, seek out others, and find out what it is they desire. I will guide you along the way.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "I expect you to recruit at least 6 family members in the next 3 days.",
                sfxName = SfxEnum.None,
            },
            new DialogueSceneText {
                name = "Tomathotep",
                text = "That will be all. I entrust this task to you, My dear friend. May your faith be resolute.",
                sfxName = SfxEnum.VLDoNotDisappointMe,
            },

        };

        private bool isBlockAction;
        public void NextDialogue()
        {
            if (isBlockAction) return;
            if (!coroutineHasStartedAtLeastOnce) return;
            if (!isTyping)
            {
                var nextIndex = currentIndex + 1;
                if (nextIndex < dialogues.Count)
                {
                    c = StartCoroutine(ShowDialogue(nextIndex));
                    SfxController.Instance.PlaySfx(dialogues[nextIndex].sfxName);
                    currentIndex = nextIndex;
                }
                else
                {
                    NextScene();
                }
            }
            else 
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = dialogues[currentIndex].text;
            }
        }

        public void PrevDialogue()
        {
            
            if (isBlockAction) return;
            if (!coroutineHasStartedAtLeastOnce) return;
            if (!isTyping)
            {
                var prevIndex = currentIndex - 1;
                if (prevIndex >= 0)
                {
                    c = StartCoroutine(ShowDialogue(prevIndex));
                    currentIndex = prevIndex;
                }
            }
            else 
            {
                isTyping = false;
                StopCoroutine(c);
                dialogueText.text = dialogues[currentIndex].text;
            }
        }

        public void NextScene()
        {
            isBlockAction = true;
            fadeUIController.FadeOutScreen(
                () => SceneManager.LoadScene(SceneNameConstant.TOWN_PLAZA)
            );
        }

        private IEnumerator ShowDialogue(int index)
        {
            isTyping = true;
            nameParent.gameObject.SetActive(true);
            nameText.text = dialogues[index].name;
            var text = dialogues[index].text;
            var curLetterIdx = 0;
            dialogueParent.gameObject.SetActive(true);
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