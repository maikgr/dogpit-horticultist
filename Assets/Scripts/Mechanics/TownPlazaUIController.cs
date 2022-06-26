namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using DG.Tweening;
    using TMPro;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.UI;

    public class TownPlazaUIController : MonoBehaviour
    {
        [SerializeField] private TransitionScreenUIController transitionScreen;
        [SerializeField] private FadeUIController fadeUIController;
        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private TownPlazaCameraController cameraController;
        private void Awake()
        {
            gameInput = new HorticultistInputActions();
            mainCamera = Camera.main;
            cameraController = mainCamera.GetComponent<TownPlazaCameraController>();
        }

        private void Start()
        {
            if (GameStateController.Instance.PrevScene == SceneNameConstant.THERAPY)
            {
                transitionScreen.TransitionOut(() => ShowTutorial());
                TownPlazaGameController.Instance.AddAction();
            }
            else
            {
                fadeUIController.FadeInScreen(() => ShowTutorial());
            }


        }

        private void OnEnable()
        {
            TownEventBus.Instance.OnDayStart += UpdateWeekDayUI;
            TownEventBus.Instance.OnDayEnd += UpdateOnDayEnd;
            TownEventBus.Instance.OnActionTaken += UpdateActionUI;

            TownEventBus.Instance.OnObedienceLevelChange += UpdateObdLevelUI;

            gameInput.Player.Fire.performed += OnClickPerformed;
            gameInput.Player.Fire.Enable();

            gameInput.UI.RightClick.performed += OnRightClickPerformed;
            gameInput.UI.RightClick.Enable();

            gameInput.UI.Point.performed += OnPointPerformed;
            gameInput.UI.Point.Enable();

        }

        private void OnDisable()
        {
            TownEventBus.Instance.OnDayStart -= UpdateWeekDayUI;
            TownEventBus.Instance.OnDayEnd -= UpdateOnDayEnd;
            TownEventBus.Instance.OnActionTaken -= UpdateActionUI;

            TownEventBus.Instance.OnObedienceLevelChange -= UpdateObdLevelUI;

            gameInput.Player.Fire.performed -= OnClickPerformed;
            gameInput.Player.Fire.Disable();

            gameInput.UI.RightClick.performed -= OnRightClickPerformed;
            gameInput.UI.RightClick.Disable();

            gameInput.UI.Point.performed -= OnPointPerformed;
            gameInput.UI.Point.Disable();

            DeselectNpc();
        }

        [Header("Action UI")]
        [SerializeField] private TMP_Text weekDayText;
        [SerializeField] private Image dayFillMask;

        private void UpdateWeekDayUI(int weekNumber, int dayNumber)
        {
            var day = weekNumber * GameStateController.Instance.DaysPerAssessment + dayNumber;
            weekDayText.text = $"Day {day}";
        }

        private bool isEnding = false;
        private void UpdateOnDayEnd(int weekNumber, int dayNumber)
        {
            // Prevent spam
            if (isEnding) return;
            isEnding = true;
            
            fadeUIController.FadeOutScreen(() => {
                SceneManager.LoadScene(SceneNameConstant.TREE_GROWTH);
            });
        }

        private void UpdateActionUI(int taken, int max)
        {
            dayFillMask.fillAmount = (float)taken / (float)max;
        }

        [Header("NPC Basic Info UI")]
        private bool panelIsOpen;
        [SerializeField] private RectTransform npcInfoPanel;
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcDialogueText;

        [Header("NPC Visual UI")]
        [SerializeField] private Image npcBodyImage;
        [SerializeField] private Image npcHeadgearImage;
        [SerializeField] private Image npcEyesImage;
        [SerializeField] private Image npcMouthImage;

        [Header("NPC Buttons UI")]
        [SerializeField] private RectTransform npcCultistButtonSet;
        [SerializeField] private Button npcSacrificeButton;
        [SerializeField] private Button npcHelpButton;
        [SerializeField] private Button npcConvertButton;
        [SerializeField] private Button scoldButton;
        [SerializeField] private Button praiseButton;

        [Header("NPC Obedience UI")]
        [SerializeField] private RectTransform cultistObediencePanel;
        [SerializeField] private TMP_Text npcObedienceText;
        [SerializeField] private List<Image> obediencePots;
        [SerializeField] private Image obedienceLeaf;

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (isActionBlock) return;
            var worldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var npc = hit.collider.GetComponent<NpcController>();
                if (npc != null)
                {
                    SelectNpc(npc);
                    SfxController.Instance.PlaySfx(SfxEnum.ClickSelect);
                    OpenPanel();
                }
            }
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            if (isActionBlock) return;
            // If button is pressed
            if (context.ReadValue<float>() > 0 && !panelIsOpen)
            {
                SfxController.Instance.PlaySfx(SfxEnum.ClickUnselect);
                ClosePanel();
            }
        }

        private NpcController pointedNpc;
        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            if (isActionBlock) return;
            var worldPos = mainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var npc = hit.collider.GetComponent<NpcController>();
                if (npc != null && !npc.IsHovered)
                {
                    npc.SetHovered();
                    pointedNpc = npc;
                }
                else if (pointedNpc != null && pointedNpc != npc)
                {
                    pointedNpc.UnsetHovered();
                    pointedNpc = null;
                }
            }
        }

        private void OpenPanel()
        {
            if (panelIsOpen) return;
            npcInfoPanel.gameObject.SetActive(true);
            DOVirtual.Float(275, 0, 0.25f, (val) =>
            {
                npcInfoPanel.anchoredPosition = new Vector2(val, npcInfoPanel.anchoredPosition.y);
            })
            .SetEase(Ease.Linear)
            .OnComplete(() => 
                {
                    panelIsOpen = true;
                    ShowPanelTutorial();
                });
        }

        public void ClosePanel()
        {
            DOVirtual.Float(0, 275, 0.25f, (val) =>
            {
                npcInfoPanel.anchoredPosition = new Vector2(val, npcInfoPanel.anchoredPosition.y);
            })
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                DeselectNpc();
                npcInfoPanel.gameObject.SetActive(false);
                panelIsOpen = false;
            });
        }

        public void SelectNpc(NpcController npc)
        {
            // Make sure previous NPC is unselected
            DeselectNpc();
            GameStateController.Instance.SetSelectedNpc(npc);
            cameraController.TrackNpc(npc);

            // Info section
            npcTypeText.text = npc.NpcType.ToString();
            npcNameText.text = npc.DisplayName;
            if (npc.NpcType == NpcTypeEnum.Cultist)
            {
                npcObedienceText.enabled = true;
                cultistObediencePanel.gameObject.SetActive(true);
                UpdateObdLevelUI(npc.ObedienceLevel);
            }
            else
            {
                cultistObediencePanel.gameObject.SetActive(false);
                npcObedienceText.enabled = false;
            }

            // Dialogues
            string dialogueText = string.Empty;
            if (npc.NpcType.Equals(NpcTypeEnum.Visitor))
            {
                dialogueText = npc.DialogueSet.visitor.GetRandom();
            }
            else if (npc.NpcType.Equals(NpcTypeEnum.Cultist))
            {
                dialogueText = npc.ObedienceDialogue;
            }
            else if (npc.NpcType.Equals(NpcTypeEnum.Townspeople) && npc.moodType.Equals(MoodEnum.Angry))
            {
                dialogueText = npc.DialogueSet.angry_person.GetRandom();
            }
            else
            {
                dialogueText = npc.DialogueSet.happy_person.GetRandom();
            }
            npcDialogueText.text = dialogueText;

            // Visual
            SetVisual(npcBodyImage, npc.bodySprite);
            SetVisual(npcEyesImage, npc.eyesSprite);
            SetVisual(npcMouthImage, npc.mouthSprite);
            SetVisual(npcHeadgearImage, npc.headgearSprite);
            npc.SetHighlighted();

            // Prepare conditional interaction by disabling all buttons
            npcHelpButton.gameObject.SetActive(false);
            npcConvertButton.gameObject.SetActive(false);
            npcCultistButtonSet.gameObject.SetActive(false);
            npcSacrificeButton.gameObject.SetActive(false);

            // Visitor buttons
            if (npc.NpcType == NpcTypeEnum.Visitor)
            {
                if (GameStateController.Instance.PlayerHasConverted)
                {
                    npcConvertButton.gameObject.SetActive(true);
                    npcConvertButton.onClick.AddListener(() => StartHelp(npc));
                }
                else
                {
                    npcHelpButton.gameObject.SetActive(true);
                    npcHelpButton.onClick.AddListener(() => StartHelp(npc));
                }
            }

            // Cultist buttons
            else if (npc.NpcType == NpcTypeEnum.Cultist)
            {
                if (npc.CultistRank == CultistRankEnum.Rank2 || npc.CultistRank == CultistRankEnum.Rank3)
                {
                    npcSacrificeButton.gameObject.SetActive(true);
                    npcSacrificeButton.onClick.AddListener(() => NpcSacrificeHandler(npc));
                }

                if (npc.HasObedienceAction)
                {
                    npcCultistButtonSet.gameObject.SetActive(true);
                    praiseButton.onClick.AddListener(() => {
                        npc.AnswerObedienceAction(CultistObedienceActionEnum.Praise);
                        npcCultistButtonSet.gameObject.SetActive(false);
                        dialogueText = npc.ObedienceDialogue;
                    });
                    scoldButton.onClick.AddListener(() => {
                        npc.AnswerObedienceAction(CultistObedienceActionEnum.Scold);
                        npcCultistButtonSet.gameObject.SetActive(false);
                        dialogueText = npc.ObedienceDialogue;
                    });
                }
            }

            // Debugging
            SetTestButton(npc);
        }

        private void SetVisual(Image imageSlot, Sprite sprite)
        {
            imageSlot.sprite = sprite;
            imageSlot.enabled = sprite != null;
        }

        private bool isActionBlock;
        private void NpcSacrificeHandler(NpcController npc)
        {
            npcSacrificeButton.gameObject.SetActive(false);
            var npcPath = npc.GetComponent<NpcAIPathfinding>();
            npcPath.StopPathfinding();
            ClosePanel();
            if (GameStateController.Instance.SacrificedMembers.Count == 0)
            {
                TownPlazaBgmController.Instance.PauseBgm();
                SfxController.Instance.PlaySfx(SfxEnum.SacrificeFirst);
                isActionBlock = true;
                cameraController.ZoomToNpc(npc);
                cameraController.TrackNpc(npc);
                
                npc.Sacrifice(() => {
                    isActionBlock = false;
                    cameraController.StopTrackNpc();
                    cameraController.ResetZoomToDefault(6f);
                    TownPlazaBgmController.Instance.ResumeBgm(6f);
                });
            }
            else
            {
                npc.Sacrifice();
                SfxController.Instance.PlaySfx(SfxEnum.Sacrifice);

            }
        }

        private void DeselectNpc()
        {
            if (GameStateController.Instance.SelectedNpc != null)
            {
                GameStateController.Instance.SelectedNpc.UnsetHighlighted();
            }
            cameraController.StopTrackNpc();
            npcHelpButton.onClick.RemoveAllListeners();
            npcConvertButton.onClick.RemoveAllListeners();
            praiseButton.onClick.RemoveAllListeners();
            scoldButton.onClick.RemoveAllListeners();
            npcSacrificeButton.onClick.RemoveAllListeners();

            // Debugging
            happyButton.onClick.RemoveAllListeners();
            angryButton.onClick.RemoveAllListeners();
            cultistButton.onClick.RemoveAllListeners();
            obedienceAddButton.onClick.RemoveAllListeners();
            obedienceSubButton.onClick.RemoveAllListeners();
        }

        public void StartHelp(NpcController npc)
        {
            DOTween.Kill("npc");
            transitionScreen.TransitionIn(
                () => StartCoroutine(LoadTherapyScene())
            );
        }

        private IEnumerator LoadTherapyScene()
        {
            var asyncLoad = SceneManager.LoadSceneAsync(SceneNameConstant.THERAPY, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        private void UpdateObdLevelUI(CultistObedienceLevelEnum obdLevel)
        {
            int obdPotIndex = 2;
            switch(obdLevel)
            {
                case CultistObedienceLevelEnum.VeryRebellious:
                    obdPotIndex = 0;
                    break;
                case CultistObedienceLevelEnum.Rebellious:
                    obdPotIndex = 1;
                    break;
                case CultistObedienceLevelEnum.Neutral:
                default:
                    obdPotIndex = 2;
                    break;
                case CultistObedienceLevelEnum.Obedient:
                    obdPotIndex = 3;
                    break;
                case CultistObedienceLevelEnum.VeryObedient:
                    obdPotIndex = 4;
                    break;
            }
            var activePotPos = obediencePots[obdPotIndex].transform.position;
            npcObedienceText.text = obdLevel.DisplayString();
            obedienceLeaf.transform.position = new Vector2(activePotPos.x, obedienceLeaf.transform.position.y);
        }

        [Header("Debugging")]
        [SerializeField] private TestTherapyController testTherapy;
        [SerializeField] private Button happyButton;
        [SerializeField] private Button angryButton;
        [SerializeField] private Button cultistButton;
        [SerializeField] private Button obedienceAddButton;
        [SerializeField] private Button obedienceSubButton;

        private void SetTestButton(NpcController npc)
        {
            happyButton.onClick.AddListener(() => testTherapy.ConvertToHappy(npc));
            angryButton.onClick.AddListener(() => testTherapy.ConvertToAngry(npc));
            cultistButton.onClick.AddListener(() => testTherapy.ConvertToCultist(npc));
            obedienceAddButton.onClick.AddListener(() => testTherapy.IncreaseObedience(npc));
            obedienceSubButton.onClick.AddListener(() => testTherapy.DecreaseObedience(npc));
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
        
        public void ShowTutorial()
        {

            var npc = GameStateController.Instance.SelectedNpc;
            var tutorial = TutorialStateVariables.Instance;
            Debug.Log(tutorial);
            if (!tutorial.HaveShownFirstTimePlaza)
            {
                FirstTimePlazaTutorial.gameObject.SetActive(true);
                FirstTimePlazaTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimePlaza = true;
                });
            }
            if (!tutorial.HaveShownFirstSuccessfulRecruit && tutorial.CanShowFirstSuccessfulRecruit)
            {
                FirstTimeSuccessfulRecruitTutorial.gameObject.SetActive(true);
                FirstTimeSuccessfulRecruitTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstSuccessfulRecruit = true;
                });
            }
            if (!tutorial.HaveShownFirstTimeSuccessfulTherapy && tutorial.CanShowFirstTimeSuccessfulTherapy)
            {
                FirstTimeSuccessfulTherapyTutorial.gameObject.SetActive(true);
                FirstTimeSuccessfulTherapyTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeSuccessfulTherapy = true;
                });
            }
            if (!tutorial.HaveShownFirstTimeFailedTherapy && tutorial.CanShowFirstTimeFailedTherapy)
            {
                FirstTimeFailedTherapyTutorial.gameObject.SetActive(true);
                FirstTimeFailedTherapyTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeFailedTherapy = true;
                });
            }
            if (!tutorial.HaveShownStartDayTwo && GameStateController.Instance.CultMembers.Count > 0 && GameStateController.Instance.DayNumber >= 2)
            {
                StartDayTwoTutorial.gameObject.SetActive(true);
                StartDayTwoTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownStartDayTwo = true;
                });
            }
        }

        public void ShowPanelTutorial()
        {
            var npc = GameStateController.Instance.SelectedNpc;
            var tutorial = TutorialStateVariables.Instance;
            if (!tutorial.HaveShownFirstTimeScoldPraise && npc.HasObedienceAction)
            {
                FirstTimeScoldPraiseTutorial.gameObject.SetActive(true);
                FirstTimeScoldPraiseTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeScoldPraise = true;
                });
            }
            if (!tutorial.HaveShownFirstTimeSacrifice && (npc.CultistRank == CultistRankEnum.Rank2 || npc.CultistRank == CultistRankEnum.Rank3))
            {
                FirstTimeSacrificeTutorial.gameObject.SetActive(true);
                FirstTimeSacrificeTutorial.OnTutorialComplete(() => {
                    TutorialStateVariables.Instance.HaveShownFirstTimeSacrifice = true;
                });
            }
        }


    }
}