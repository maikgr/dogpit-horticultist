namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;
    using Horticultist.Scripts.Core;

    public class TutorialDialogueController : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text nextButtonText;
        [SerializeField] private List<DialogueSceneText> dialogues;
        private System.Action onTutorialComplete;

        private void OnEnable()
        {
            // Pause the game
            Time.timeScale = 0;
            ShowDialogue(curIndex);
            nextButtonText.text = "Next";

            if (dialogues.Count == 0)
            {
                SkipTutorial();
            }
        }

        private int curIndex;
        public void NextDialogue()
        {
            curIndex = Mathf.Clamp(curIndex + 1, 0, dialogues.Count);
            // Change next button to "Done" on last dialogue
            if (curIndex == dialogues.Count - 1)
            {
                nextButtonText.text = "Done!";
            }
            
            // Show dialogue if index within list
            if (curIndex < dialogues.Count)
            {
                ShowDialogue(curIndex);
            }
            // Exit tutorial when index exceeds count
            else
            {
                SkipTutorial();
            }
        }

        public void PrevDialogue()
        {
            curIndex = Mathf.Clamp(curIndex - 1, 0, dialogues.Count - 1);
            // Change next button to "Next" if not last dialogue
            if (curIndex < dialogues.Count - 1)
            {
                nextButtonText.text = "Next";
            }
            ShowDialogue(curIndex);
        }

        public void OnTutorialComplete(System.Action onComplete)
        {
            onTutorialComplete = onComplete;
        }

        public void SkipTutorial()
        {
            if (onTutorialComplete != null)
            {
                onTutorialComplete.Invoke();
            }
            Time.timeScale = 1;
            this.gameObject.SetActive(false);
        }

        private void ShowDialogue(int index)
        {
            nameText.text = dialogues[index].name;
            dialogueText.text = dialogues[index].text;
        }
    }
}
