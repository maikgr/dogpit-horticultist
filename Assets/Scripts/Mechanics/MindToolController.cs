namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;

    public class MindToolController : MonoBehaviour
    {
        public static MindToolController Instance { get; private set; }
        [SerializeField] private ToolTypeEnum activeToolType;

        public ToolTypeEnum ActiveToolType => activeToolType;

        private void Awake() {
            var controllers = GameObject.FindObjectsOfType<MindToolController>();
            if (controllers.Length > 1)
            {
                Debug.LogError("Only one MindToolController can be active!");
            }

            Instance = this;
        }

        public void SetWrenchTool()
        {
            SetTool(ToolTypeEnum.Wrench);
        }
        public void SetBroomTool()
        {
            SetTool(ToolTypeEnum.Broom);
        }
        public void SetTowelTool()
        {
            SetTool(ToolTypeEnum.CleaningTowel);
        }
        public void SetTomatoTool()
        {
            SetTool(ToolTypeEnum.Tomato);
        }
        public void SetPrayerTool()
        {
            SetTool(ToolTypeEnum.Pray);
        }

        private void SetTool(ToolTypeEnum tool)
        {
            if (activeToolType == tool)
            {
                UnsetTool();
                SfxController.Instance.PlaySfx(SfxEnum.ToolUnselect);
            }
            else
            {
                activeToolType = tool;
                SfxController.Instance.PlaySfx(SfxEnum.ToolSelect);
            }
        }

        private void UnsetTool()
        {
            activeToolType = ToolTypeEnum.None;
        }
    }
}

