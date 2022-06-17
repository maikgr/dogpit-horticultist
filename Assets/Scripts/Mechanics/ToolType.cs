namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class ToolType : MonoBehaviour
    {

        [SerializeField] private ToolTypeEnum thisToolType;
        public ToolTypeEnum ThisToolType => thisToolType;

    }
}