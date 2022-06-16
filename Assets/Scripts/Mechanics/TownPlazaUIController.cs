namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.InputSystem;
    using UnityEngine.SceneManagement;
    using UnityEngine.EventSystems;
    using DG.Tweening;
    using TMPro;
    using Horticultist.Scripts.Core;

    public class TownPlazaUIController : MonoBehaviour
    {
        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private void Awake()
        {
            gameInput = new HorticultistInputActions();
            mainCamera = Camera.main;
        }

        private void OnEnable() {
            TownEventBus.Instance.OnDayStart += UpdateWeekDayUI;
            TownEventBus.Instance.OnActionTaken += UpdateActionUI;

            gameInput.Player.Fire.performed += OnClickPerformed;
            gameInput.Player.Fire.Enable();

            gameInput.UI.RightClick.performed += OnRightClickPerformed;
            gameInput.UI.RightClick.Enable();
        }

        private void OnDisable()
        {
            TownEventBus.Instance.OnDayStart -= UpdateWeekDayUI;
            TownEventBus.Instance.OnActionTaken -= UpdateActionUI;

            gameInput.Player.Fire.performed -= OnClickPerformed;
            gameInput.Player.Fire.Disable();
            
            gameInput.UI.RightClick.performed -= OnRightClickPerformed;
            gameInput.UI.RightClick.Disable();  

            npcHelpButton.onClick.RemoveAllListeners();
            npcConvertButton.onClick.RemoveAllListeners();

            happyButton.onClick.RemoveAllListeners();
            angryButton.onClick.RemoveAllListeners();
            cultistButton.onClick.RemoveAllListeners();
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
            dayFillMask.fillAmount = (float)taken/(float)max;
        }
        
        [Header("NPC UI")]
        [SerializeField] private GraphicRaycaster graphicRaycaster;
        [SerializeField] private RectTransform npcInfoPanel;
        [SerializeField] private TMP_Text npcTypeText;
        [SerializeField] private TMP_Text npcNameText;
        [SerializeField] private TMP_Text npcObedienceText;
        [SerializeField] private TMP_Text npcDialogueText;
        [SerializeField] private Image npcBodyImage;
        [SerializeField] private Image npcHeadgearImage;
        [SerializeField] private Image npcEyesImage;
        [SerializeField] private Image npcMouthImage;
        [SerializeField] private RectTransform npcCultistButtonSet;
        [SerializeField] private Button npcSacrificeButton;
        [SerializeField] private Button npcHelpButton;
        [SerializeField] private Button npcConvertButton;

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
            npcInfoPanel.gameObject.SetActive(true);
            DOVirtual.Float(275, 0, 0.25f, (val) =>
            {
                npcInfoPanel.anchoredPosition = new Vector2(val, npcInfoPanel.anchoredPosition.y);
            })
            .SetEase(Ease.Linear);
        }

        public void ClosePanel()
        {
            DOVirtual.Float(0, 275, 0.25f, (val) =>
            {
                npcInfoPanel.anchoredPosition = new Vector2(val, npcInfoPanel.anchoredPosition.y);
            })
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                DeselectNpc();
                npcInfoPanel.gameObject.SetActive(false);
            });
        }

        public void SelectNpc(NpcController npc)
        {
            // Info section
            npcTypeText.text = npc.NpcType.ToString();
            npcNameText.text = npc.DisplayName;
            if (npc.NpcType == NpcTypeEnum.Cultist)
            {
                npcObedienceText.enabled = true;
                npcObedienceText.text = npc.ObedienceLevel;
            }
            else
            {
                npcObedienceText.enabled = false;
            }
            npcDialogueText.text = "Test";

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
                npcCultistButtonSet.gameObject.SetActive(true);
                if (npc.NpcType == NpcTypeEnum.Cultist && npc.ObedienceValue > 5)
                {
                    npcSacrificeButton.gameObject.SetActive(true);
                }
            }

            // Debugging
            SetTestButton(npc);
        }

        private void DeselectNpc()     {
            npcHelpButton.onClick.RemoveAllListeners();
            npcConvertButton.onClick.RemoveAllListeners();
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

        [Header("Debugging")]
        [SerializeField] private TestTherapyController testTherapy;
        [SerializeField] private Button happyButton;
        [SerializeField] private Button angryButton;
        [SerializeField] private Button cultistButton;

        private void SetTestButton(NpcController npc)
        {
            happyButton.onClick.AddListener(() => testTherapy.ConvertToHappy(npc));
            angryButton.onClick.AddListener(() => testTherapy.ConvertToAngry(npc));
            cultistButton.onClick.AddListener(() => testTherapy.ConvertToCultist(npc));
        }
    }
}