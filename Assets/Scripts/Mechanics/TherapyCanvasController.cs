namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using UnityEngine.InputSystem;
    using TMPro;
    using DG.Tweening;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;

    public class TherapyCanvasController : MonoBehaviour
    {
        [Header("Effects and Transition")]
        [SerializeField] private TransitionScreenUIController transitionScreen;
        [SerializeField] private ProgressBarVisual progressVisual;
        [SerializeField] private TherapyMoodEmoteController happyMoodBubble;
        [SerializeField] private TherapyMoodEmoteController angryMoodBubble;
        [SerializeField] private TherapyMoodEmoteController sparkleMoodBubble;
        [SerializeField] private TherapyMoodEmoteController tomatoMoodBubble;

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
        private HorticultistInputActions gameInput;

        private void Awake() {
            progressVisual.progressBar.gameObject.SetActive(false);
            gameInput = new HorticultistInputActions();
        }
        
        private void Start() {
            currentNpc = GameStateController.Instance.SelectedNpc;

            // Basic info
            npcTypeText.text = currentNpc.NpcType.ToString();
            npcNameText.text = currentNpc.DisplayName;
            npcDialogueText.text = currentNpc.DialogueSet.therapy.intro.GetRandom();

            // Visuals
            UpdateNpcVisual();

            // Paremeters
            UpdateNpcParemeters();

            // Force material reimport before transitioning
            transitionScreen.enabled = false;
            transitionScreen.enabled = true;
            transitionScreen.TransitionOut(() => ShowTutorial());
            GameStateController.Instance.PrevScene = SceneNameConstant.THERAPY;
        }

        private void OnEnable() {
            TherapyEventBus.Instance.OnMoodChanged += OnMoodChanged;
            TherapyEventBus.Instance.OnPatienceChanged += OnPatienceChanged;
            TherapyEventBus.Instance.OnIndoctrinationChanged += OnIndoctrinationChanged;
            TherapyEventBus.Instance.OnTherapyEnds += OnTherapyEnds;

            gameInput.UI.Click.performed += OnClickPerformed;
            gameInput.UI.Click.Enable();
        }

        private void OnDisable() {
            TherapyEventBus.Instance.OnMoodChanged -= OnMoodChanged;
            TherapyEventBus.Instance.OnPatienceChanged -= OnPatienceChanged;
            TherapyEventBus.Instance.OnIndoctrinationChanged -= OnIndoctrinationChanged;
            TherapyEventBus.Instance.OnTherapyEnds -= OnTherapyEnds;

            gameInput.UI.Click.performed -= OnClickPerformed;
            gameInput.UI.Click.Disable();
        }

        private void UpdateNpcVisual()
        {
            SetVisual(npcBodyImage, currentNpc.bodySprite);
            SetVisual(npcEyesImage, currentNpc.eyesSprite);
            SetVisual(npcMouthImage, currentNpc.mouthSprite);
            SetVisual(npcHeadgearImage, currentNpc.headgearSprite);
        }
        
        private void SetVisual(Image imageSlot, Sprite sprite)
        {
            imageSlot.sprite = sprite;
            imageSlot.enabled = sprite != null;
        }

        private void UpdateNpcParemeters()
        {
            indoctrinationbar.fillAmount = currentNpc.IndoctrinationValue / 100f;
            patienceBar.fillAmount = currentNpc.PatienceValue / 100f;
        }

        private TherapyMoodEmoteController curMood;
        private bool isFinalMood;
        private void OnMoodChanged(MoodEnum mood)
        {
            if (currentNpc.moodType != mood) {
                if (mood == MoodEnum.Angry)
                {
                    SfxController.Instance.PlaySfx(SfxEnum.TherapyMoodChangeBad);
                }
                else
                {
                    SfxController.Instance.PlaySfx(SfxEnum.TherapyMoodChangeGood);
                }
            }

            if (mood == MoodEnum.Angry)
            {
                npcDialogueText.text = currentNpc.DialogueSet.therapy.mooddown.GetRandom();
                if (!isFinalMood) curMood = angryMoodBubble;
            }
            else
            {
                npcDialogueText.text = currentNpc.DialogueSet.therapy.moodup.GetRandom();
                if (!isFinalMood) curMood = happyMoodBubble;
            }

            currentNpc.SetMood(mood);
            UpdateNpcVisual();
        }

         // [prev, current]
        private ClutterWorkEvent[] clutterWorkEvents = new ClutterWorkEvent[2];
        private bool isShowProgressBar;
        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (context.ReadValue<float>() > 0)
            {
                isShowProgressBar = true;

                // Update progress bar visual
                progressVisual.progressBar.rectTransform.position = Mouse.current.position.ReadValue() + progressVisual.offset;
            }
            else
            {
                isShowProgressBar = false;
                progressVisual.progressBar.gameObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (isShowProgressBar && clutterWorkEvents[1] != null && clutterWorkEvents[0] != clutterWorkEvents[1])
            {
                clutterWorkEvents[0] = clutterWorkEvents[1];
                var workEvent = clutterWorkEvents[0];
                progressVisual.progressBar.gameObject.SetActive(true);

                // Move progress bar fill
                var totalTime = workEvent.totalWorkTime + 1;
                var fromVal = Mathf.Min(workEvent.remainingWorkTime + 2, totalTime);
                var toVal = Mathf.Min(workEvent.remainingWorkTime + 1, totalTime);
                DOVirtual.Float(fromVal, toVal, 0.75f, (value) => {
                    progressVisual.progressBar.fillAmount = value/totalTime;
                });

                if (toVal == 0 && workEvent.isIndoctrinationTool)
                {
                    isFinalMood = true;
                    curMood = tomatoMoodBubble;
                }
                else if (toVal == 0 && !workEvent.isIndoctrinationTool)
                {
                    isFinalMood = true;
                    curMood = sparkleMoodBubble;
                }
            }

            // Show completion emote bubble
            if (curMood != null)
            {
                curMood.gameObject.SetActive(true);
                curMood.ShowEmote(Mouse.current.position.ReadValue());
                curMood = null;
                isFinalMood = false;
            }
        }

        private void OnPatienceChanged(ClutterWorkEvent workEvent)
        {
            // Update patience parameter
            currentNpc.SetPatience(Mathf.Max(currentNpc.PatienceValue - workEvent.amountChanged, 0));
            UpdateNpcParemeters();
            
            // Update progress bar visual
            clutterWorkEvents[1] = workEvent;
            progressVisual.progressBar.color = progressVisual.cleaningProgressColor;
        }

        private void OnIndoctrinationChanged(ClutterWorkEvent workEvent)
        {
            currentNpc.SetIndoctrination(Mathf.Min(currentNpc.IndoctrinationValue + workEvent.amountChanged, 100));
            UpdateNpcParemeters();
            clutterWorkEvents[1] = workEvent;

            // Update progress bar visual
            clutterWorkEvents[1] = workEvent;
            progressVisual.progressBar.color = progressVisual.indoctrinationProgressColor;
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
                npcDialogueText.text = currentNpc.DialogueSet.therapy.failure.GetRandom();
            }
            else if (npcType == NpcTypeEnum.Cultist)
            {
                npcDialogueText.text = currentNpc.DialogueSet.therapy.success.GetRandom();
            }
            else
            {
                npcDialogueText.text = currentNpc.DialogueSet.therapy.unrecruited.GetRandom();
            }
            StartCoroutine(LoadTownPlaza(2f));
        }

        private IEnumerator LoadTownPlaza(float delay)
        {
            yield return new WaitForSeconds(delay);
            transitionScreen.TransitionIn(() => SceneManager.LoadScene(SceneNameConstant.TOWN_PLAZA));
        }

        [Header("Tutorials")]
        [SerializeField] private TutorialDialogueController FirstTimePlazaTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeTherapyTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeWealthTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeHealthTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeLoveTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeSuccessfulRecruitTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeSuccessfulTherapyTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeFailedTherapyTutorial; 
        [SerializeField] private TutorialDialogueController StartDayTwoTutorial;
        [SerializeField] private TutorialDialogueController FirstTimeScoldPraiseTutorial; 
        [SerializeField] private TutorialDialogueController FirstTimeSacrificeTutorial; 

        private void ShowTutorial()
        {

            var tutorial = TutorialStateVariables.Instance;
            var personality = currentNpc.npcPersonality;

            if (!tutorial.HaveShownFirstTimeTherapy)
            {
                FirstTimeTherapyTutorial.gameObject.SetActive(true);
                FirstTimeTherapyTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeTherapy = true;
                });
            }

            if (!tutorial.HaveShownFirstTimeWealth && personality == NpcPersonalityEnum.Wealth)
            {
                FirstTimeWealthTutorial.gameObject.SetActive(true);
                FirstTimeWealthTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeWealth = true;
                });
            }
            else if (!tutorial.HaveShownFirstTimeHealth && personality == NpcPersonalityEnum.Health)
            {
                FirstTimeHealthTutorial.gameObject.SetActive(true);
                FirstTimeHealthTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeHealth = true;
                });
            }
            else if (!tutorial.HaveShownFirstTimeLove && personality == NpcPersonalityEnum.Love)
            {
                FirstTimeLoveTutorial.gameObject.SetActive(true);
                FirstTimeLoveTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeLove = true;
                });
            }
        }

    }

    [System.Serializable]
    public class ProgressBarVisual
    {
        public Image progressBar;
        public Vector2 offset;
        public Color indoctrinationProgressColor;
        public Color cleaningProgressColor;
    }
}