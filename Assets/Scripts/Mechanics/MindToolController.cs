namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

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
    }
}

