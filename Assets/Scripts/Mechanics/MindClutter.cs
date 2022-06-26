namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;


    public class MindClutter : MonoBehaviour
    {

        [SerializeField] private List<ToolClutterInteraction> toolInteractions;
        [SerializeField] private int cleaningWorkTime;
        private int totalCleaningTime;
        [SerializeField] private int indoctrinationWorkTime;
        private int totalIndoctrinationTime;
        [SerializeField] private SpriteRenderer clutterSpriteRenderer;
        [SerializeField] private Sprite dirtySprite;
        [SerializeField] private Sprite cleanSprite;
        [SerializeField] private Sprite indoctrinatedSprite;
        [SerializeField] private Sprite highlightSprite;

        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private Dictionary<ToolTypeEnum, ToolClutterInteraction> availableToolMap;
        public ClutterStateEnum CurrentState { get; private set; }
        private bool isButtonPressed = false;
        private bool isIndoctrinatedToolSelected = false;
        public Action onInteracted;
        public Action onStateChange;

        private void Awake() {
            totalCleaningTime = cleaningWorkTime;
            totalIndoctrinationTime = indoctrinationWorkTime;
        }

        void Update() {
            // if (cleaningWorkTime <= 0 && CurrentState == ClutterStateEnum.Dirty) {
            //     isButtonPressed = false;
            //     StopCoroutine(UpdateValues(null));
            //     SetStateClean();
            //     SfxController.Instance.PlaySfx(SfxEnum.CleanSuccess);
            // }
            // else if (indoctrinationWorkTime <= 0 && CurrentState == ClutterStateEnum.Dirty) {
            //     isButtonPressed = false;
            //     StopCoroutine(UpdateValues(null));
            //     SfxController.Instance.PlaySfx(SfxEnum.VineSuccess);
            //     SetStateIndoctrinated();
            // }
        }

        public void OnClickDown()
        {
            var activeToolType = MindToolController.Instance.ActiveToolType;
            var isToolValid = toolInteractions.Any(t => t.MindToolType.Equals(activeToolType));
            isIndoctrinatedToolSelected = isIndoctrinatedTool(activeToolType);

            if (isToolValid && CurrentState == ClutterStateEnum.Dirty)
            {
                var values = toolInteractions.First(t => t.MindToolType.Equals(activeToolType));
                isButtonPressed = true;
                StartCoroutine(UpdateValues(values));
            }

        }

        public void OnClickUp()
        {
            isButtonPressed = false;
            StopCoroutine(UpdateValues(null));
        }

        public void OnHoverEnter()
        {
            if (CurrentState == ClutterStateEnum.Dirty)
            {
                clutterSpriteRenderer.sprite = highlightSprite;
            }
        }

        public void OnHoverExit()
        {
            if (CurrentState == ClutterStateEnum.Dirty)
            {
                clutterSpriteRenderer.sprite = dirtySprite;
            }
        }

        private IEnumerator UpdateValues(ToolClutterInteraction values)
        {
            UpdateMood(values.MoodInpact);
            while (isButtonPressed) {
                UpdatePatience(values.PatienceValue);
                UpdateIndoctrination(values.IndoctrinationValue);
                UpdateCleaning();
                if (onInteracted != null)
                {
                    onInteracted.Invoke();
                }
                yield return new WaitForSeconds(1f);
            }
        }

        public void SetStateDirty() 
        {
            CurrentState = ClutterStateEnum.Dirty;
            clutterSpriteRenderer.sprite = dirtySprite;
        }

        public void SetStateClean() 
        {
            CurrentState = ClutterStateEnum.Clean;
            clutterSpriteRenderer.sprite = cleanSprite;
            if (onStateChange != null) {
                onStateChange.Invoke();
            }
        }

        public void SetStateIndoctrinated() 
        {
            CurrentState = ClutterStateEnum.Indoctrinated;
            clutterSpriteRenderer.sprite = indoctrinatedSprite;
            if (onStateChange != null) {
                onStateChange.Invoke();
            }
            
        }

        private void UpdatePatience(int value) 
        {
            TherapyEventBus.Instance.DispatchOnPatienceChanged(
                new ClutterWorkEvent
                {
                    amountChanged = value,
                    remainingWorkTime = cleaningWorkTime,
                    totalWorkTime = totalCleaningTime
                }
            );
        }

        private void UpdateMood(MoodEnum moodImpact) 
        {
            TherapyEventBus.Instance.DispatchOnMoodChanged(moodImpact);
        }

        private void UpdateIndoctrination(int value) 
        {
            if (indoctrinationWorkTime <= 0 && CurrentState == ClutterStateEnum.Dirty) {
                isButtonPressed = false;
                StopCoroutine(UpdateValues(null));
                SfxController.Instance.PlaySfx(SfxEnum.VineSuccess);
                SetStateIndoctrinated();
            }

            if (isIndoctrinatedToolSelected) {
                indoctrinationWorkTime -= 1;
                TherapyEventBus.Instance.DispatchOnIndoctrinationChanged(
                    new ClutterWorkEvent
                    {
                        amountChanged = value,
                        remainingWorkTime = indoctrinationWorkTime,
                        totalWorkTime = totalIndoctrinationTime
                    }
                );
            }
        }

        private void UpdateCleaning() 
        {
            if (cleaningWorkTime <= 0 && CurrentState == ClutterStateEnum.Dirty) {
                isButtonPressed = false;
                StopCoroutine(UpdateValues(null));
                SetStateClean();
                SfxController.Instance.PlaySfx(SfxEnum.CleanSuccess);
            }
            if (!isIndoctrinatedToolSelected) {
                cleaningWorkTime -= 1;
            }

        }

        // Utils
        public bool isIndoctrinatedTool(ToolTypeEnum toolType) 
        {
            return new ToolTypeEnum[2] { ToolTypeEnum.Tomato, ToolTypeEnum.Pray}.Any(s => s == toolType);
        }
    }
}