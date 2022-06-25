namespace Horticultist.Scripts.Mechanics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;
    using Horticultist.Scripts.Core;


    public class MindClutter : MonoBehaviour
    {

        [SerializeField] private List<ToolClutterInteraction> toolInteractions;
        [SerializeField] private int cleaningWorkTime;
        [SerializeField] private int indoctrinationWorkTime;
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

        void Update() {
            if (cleaningWorkTime <= 0) {
                isButtonPressed = false;
                StopCoroutine(UpdateValues(null));
                SetStateClean();
            }
            else if (indoctrinationWorkTime <= 0) {
                isButtonPressed = false;
                StopCoroutine(UpdateValues(null));
                SetStateIndoctrinated();
            }
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
            if (onInteracted != null)
            {
                onInteracted.Invoke();
            }
        }

        public void SetStateIndoctrinated() 
        {
            CurrentState = ClutterStateEnum.Indoctrinated;
            clutterSpriteRenderer.sprite = indoctrinatedSprite;
            if (onInteracted != null)
            {
                onInteracted.Invoke();
            }
        }

        private void UpdatePatience(int value) 
        {
            TherapyEventBus.Instance.DispatchOnPatienceChanged(value);
        }

        private void UpdateMood(MoodEnum moodImpact) 
        {
            TherapyEventBus.Instance.DispatchOnMoodChanged(moodImpact);
        }

        private void UpdateIndoctrination(int value) 
        {
            if (isIndoctrinatedToolSelected) {
                indoctrinationWorkTime -= 1;
                TherapyEventBus.Instance.DispatchOnIndoctrinationChanged(value);
            }
        }

        private void UpdateCleaning() 
        {
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