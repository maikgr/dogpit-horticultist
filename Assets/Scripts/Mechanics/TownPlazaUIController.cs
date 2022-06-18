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

    public class TownPlazaUIController : MonoBehaviour
    {
        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private void Awake()
        {
            gameInput = new HorticultistInputActions();
            mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            TownEventBus.Instance.OnDayStart += UpdateWeekDayUI;
            TownEventBus.Instance.OnActionTaken += UpdateActionUI;

            TownEventBus.Instance.OnObedienceLevelChange += UpdateObdLevelUI;

            gameInput.Player.Fire.performed += OnClickPerformed;
            gameInput.Player.Fire.Enable();

            gameInput.UI.RightClick.performed += OnRightClickPerformed;
            gameInput.UI.RightClick.Enable();
        }

        private void OnDisable()
        {
            TownEventBus.Instance.OnDayStart -= UpdateWeekDayUI;
            TownEventBus.Instance.OnActionTaken -= UpdateActionUI;

            TownEventBus.Instance.OnObedienceLevelChange -= UpdateObdLevelUI;

            gameInput.Player.Fire.performed -= OnClickPerformed;
            gameInput.Player.Fire.Disable();

            gameInput.UI.RightClick.performed -= OnRightClickPerformed;
            gameInput.UI.RightClick.Disable();

            DeselectNpc();
        }

        [Header("Action UI")]
        [SerializeField] private TMP_Text weekDayText;
        [SerializeField] private Image dayFillMask;

        private void UpdateWeekDayUI(int weekNumber, int dayNumber)
        {
            weekDayText.text = $"Week {weekNumber} Day {dayNumber}";
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
            var worldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null)
            {
                var npc = hit.collider.GetComponent<NpcController>();
                if (npc != null)
                {
                    SelectNpc(npc);
                    OpenPanel();
                }
            }
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            // If button is pressed
            if (context.ReadValue<float>() > 0)
            {
                ClosePanel();
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
            .OnComplete(() => panelIsOpen = true);
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
                dialogueText = npc.DialogueSet.Visitor.GetRandom();
            }
            else if (npc.NpcType.Equals(NpcTypeEnum.Cultist))
            {
                dialogueText = npc.ObedienceDialogue;
            }
            else if (npc.NpcType.Equals(NpcTypeEnum.Townspeople) && npc.moodType.Equals(MoodTypeEnum.Angry))
            {
                dialogueText = npc.DialogueSet.Angry_person.GetRandom();
            }
            else
            {
                dialogueText = npc.DialogueSet.Happy_person.GetRandom();
            }
            npcDialogueText.text = dialogueText;

            // Visual
            npcBodyImage.sprite = npc.bodySprite;
            npcEyesImage.sprite = npc.eyesSprite;
            npcMouthImage.sprite = npc.mouthSprite;
            if (npc.headgearSprite == null)
            {
                npcHeadgearImage.enabled = false;
            }
            else
            {
                npcHeadgearImage.enabled = true;
                npcHeadgearImage.sprite = npc.headgearSprite;
            }

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
                if (npc.NpcType == NpcTypeEnum.Cultist && npc.ObedienceValue > 5)
                {
                    npcSacrificeButton.gameObject.SetActive(true);
                }
                if (npc.HasObedienceAction)
                {
                    npcCultistButtonSet.gameObject.SetActive(true);
                    praiseButton.onClick.AddListener(() => {
                        npc.ObedienceAction(CultistObedienceActionEnum.Praise);
                        npcCultistButtonSet.gameObject.SetActive(false);
                        dialogueText = npc.ObedienceDialogue;
                    });
                    scoldButton.onClick.AddListener(() => {
                        npc.ObedienceAction(CultistObedienceActionEnum.Scold);
                        npcCultistButtonSet.gameObject.SetActive(false);
                        dialogueText = npc.ObedienceDialogue;
                    });
                }
            }

            // Debugging
            SetTestButton(npc);
        }

        private void DeselectNpc()
        {
            npcHelpButton.onClick.RemoveAllListeners();
            npcConvertButton.onClick.RemoveAllListeners();
            praiseButton.onClick.RemoveAllListeners();
            scoldButton.onClick.RemoveAllListeners();

            // Debugging
            happyButton.onClick.RemoveAllListeners();
            angryButton.onClick.RemoveAllListeners();
            cultistButton.onClick.RemoveAllListeners();
            obedienceAddButton.onClick.RemoveAllListeners();
            obedienceSubButton.onClick.RemoveAllListeners();
        }

        public void StartHelp(NpcController npc)
        {
            GameStateController.Instance.SetSelectedNpc(npc);
            DOTween.Pause("npc");
            StartCoroutine(LoadTherapyScene());
        }

        private IEnumerator LoadTherapyScene()
        {
            var asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

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
    }
}