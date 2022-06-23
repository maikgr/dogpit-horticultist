namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using TMPro;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;

    public class TherapyCanvasController : MonoBehaviour
    {
        [Header("Effects and Transition")]
        [SerializeField] private TransitionScreenUIController transitionScreen;
        [SerializeField] private string townPlazaSceneName;

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
            UpdateNpcVisual();

            // Paremeters
            UpdateNpcParemeters();

            // Force material reimport before transitioning
            transitionScreen.enabled = false;
            transitionScreen.enabled = true;
            transitionScreen.TransitionOut();
        }

        private void OnEnable() {
            TherapyEventBus.Instance.OnMoodChanged += OnMoodChanged;
            TherapyEventBus.Instance.OnPatienceChanged += OnPatienceChanged;
            TherapyEventBus.Instance.OnIndoctrinationChanged += OnIndoctrinationChanged;
            TherapyEventBus.Instance.OnTherapyEnds += OnTherapyEnds;

            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDisable() {
            TherapyEventBus.Instance.OnMoodChanged -= OnMoodChanged;
            TherapyEventBus.Instance.OnPatienceChanged -= OnPatienceChanged;
            TherapyEventBus.Instance.OnIndoctrinationChanged -= OnIndoctrinationChanged;
            TherapyEventBus.Instance.OnTherapyEnds -= OnTherapyEnds;

            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene prev, Scene next)
        {
            GameStateController.Instance.PrevScene = SceneNameConstant.THERAPY;
            isEnding = false;
        }

        private void UpdateNpcVisual()
        {
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
        }

        private void UpdateNpcParemeters()
        {
            indoctrinationbar.fillAmount = currentNpc.IndoctrinationValue / 100f;
            patienceBar.fillAmount = currentNpc.PatienceValue / 100f;
        }

        private void OnMoodChanged(MoodEnum mood)
        {
            currentNpc.SetMood(mood);
            npcDialogueText.text = currentNpc.ObedienceDialogue;
            UpdateNpcVisual();
        }

        private void OnPatienceChanged(int value)
        {
            currentNpc.SetPatience(Mathf.Max(currentNpc.PatienceValue - value, 0));
            UpdateNpcParemeters();
        }

        private void OnIndoctrinationChanged(int value)
        {
            Debug.Log("UI change by " + value);
            currentNpc.SetIndoctrination(Mathf.Min(currentNpc.IndoctrinationValue + value, 100));
            UpdateNpcParemeters();
        }

        private bool isEnding;
        private void OnTherapyEnds(NpcTypeEnum npcType, MoodEnum mood)
        {
            if (isEnding) return;
            isEnding = true;
            currentNpc.SetMood(mood);
            currentNpc.ChangeType(npcType);

            if (npcType == NpcTypeEnum.Townspeople && mood == MoodEnum.Angry)
            {
                npcDialogueText.text = currentNpc.DialogueSet.Therapy.Failure.GetRandom();
            }
            else if (npcType == NpcTypeEnum.Cultist)
            {
                npcDialogueText.text = currentNpc.DialogueSet.Therapy.Success.GetRandom();
            }
            else
            {
                npcDialogueText.text = currentNpc.DialogueSet.Therapy.Unrecruited.GetRandom();
            }
            StartCoroutine(LoadTownPlaza(3f));
        }

        private IEnumerator LoadTownPlaza(float delay)
        {
            yield return new WaitForSeconds(delay);
            transitionScreen.TransitionIn(() => SceneManager.LoadScene(townPlazaSceneName));
        }
    }
}