namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Linq;
    using Horticultist.Scripts.Core;


    public class MindClutter : MonoBehaviour
    {

        [SerializeField] private List<GameObject> toolInteractions;
        [SerializeField] private int cleaningWorkTime;
        [SerializeField] private int indoctrinationWorkTime;
        [SerializeField] private SpriteRenderer clutterSpriteRenderer;
        [SerializeField] private Sprite dirtySprite;
        [SerializeField] private Sprite cleanSprite;
        [SerializeField] private Sprite indoctrinatedSprite;

        private HorticultistInputActions gameInput;
        private Camera mainCamera;
        private Dictionary<ToolTypeEnum, ToolClutterInteraction> availableToolMap;
        private ClutterStateEnum currentState;
        private bool isButtonPressed = false;
        private bool isIndoctrinatedToolSelected = false;

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

        private void Awake()
        {
            var availableToolInteractions = toolInteractions.Select((interaction) => interaction.GetComponent<ToolClutterInteraction>()).ToList();
            availableToolMap = availableToolInteractions.ToDictionary(i => i.MindToolType, i => i);  
        }


        public void OnClickDown()
        {
            var activeToolType = MindToolController.Instance.ActiveToolType;
            isIndoctrinatedToolSelected = isIndoctrinatedTool(activeToolType);

            if (availableToolMap.ContainsKey(activeToolType) && currentState == ClutterStateEnum.Dirty)
            {
                var values = availableToolMap[activeToolType];
                isButtonPressed = true;
                StartCoroutine(UpdateValues(values));
            }

        }

        public void OnClickUp()
        {
            isButtonPressed = false;
            StopCoroutine(UpdateValues(null));
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
            currentState = ClutterStateEnum.Dirty;
            clutterSpriteRenderer.sprite = dirtySprite;
        }

        public void SetStateClean() 
        {
            currentState = ClutterStateEnum.Clean;
            clutterSpriteRenderer.sprite = cleanSprite;
        }

        public void SetStateIndoctrinated() 
        {
            currentState = ClutterStateEnum.Indoctrinated;
            clutterSpriteRenderer.sprite = indoctrinatedSprite;
        }

        private void UpdatePatience(int value) 
        {
            Debug.Log("Patience " + value);
            TherapyEventBus.Instance.DispatchOnPatienceChanged(value);
        }

        private void UpdateMood(MoodEnum moodImpact) 
        {
            Debug.Log("Mood" + moodImpact);
            TherapyEventBus.Instance.DispatchOnMoodChanged(moodImpact);
        }

        private void UpdateIndoctrination(int value) 
        {
            if (isIndoctrinatedToolSelected) {
                Debug.Log("Indoctrination" + value);
                indoctrinationWorkTime -= 1;
                TherapyEventBus.Instance.DispatchOnIndoctrinationChanged(value);
            }
        }

        private void UpdateCleaning() 
        {
            if (!isIndoctrinatedToolSelected) {
                Debug.Log("Cleaning");
                cleaningWorkTime -= 1;
            }
        }

        // Utils
        public bool isIndoctrinatedTool(ToolTypeEnum toolType) 
        {
            return toolType == ToolTypeEnum.Wrench;
        }
    }
}