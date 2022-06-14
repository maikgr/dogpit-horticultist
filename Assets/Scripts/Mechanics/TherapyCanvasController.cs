namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class TherapyCanvasController : MonoBehaviour
    {
        [SerializeField] TMP_Text TestNpcText;
        
        private void Start() {
            var npc = GameStateController.Instance.SelectedNpc;

            TestNpcText.text = $"Loaded NPC: {npc.DisplayName}";
        }
    }
}