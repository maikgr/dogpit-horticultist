namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class ToolClutterInteraction : MonoBehaviour
    {
        [SerializeField] private ToolTypeEnum mindToolType;
        [SerializeField] private int patienceValue;
        [SerializeField] private int indoctrinationValue;
        [SerializeField] private MoodEnum moodInpact;

        public ToolTypeEnum MindToolType => mindToolType;
        public int PatienceValue => patienceValue;
        public int IndoctrinationValue => indoctrinationValue;
        public MoodEnum MoodInpact => moodInpact;
    }
}