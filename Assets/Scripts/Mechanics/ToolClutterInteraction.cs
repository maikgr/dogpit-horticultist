namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    [System.Serializable]
    public class ToolClutterInteraction
    {
        public ToolTypeEnum MindToolType;
        public int PatienceValue;
        public int IndoctrinationValue;
        public MoodEnum MoodInpact;
    }
}