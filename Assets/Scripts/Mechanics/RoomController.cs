namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random=System.Random;
    using Horticultist.Scripts.Core;
    using Horticultist.Scripts.UI;

    public class RoomController : MonoBehaviour
    {
        [SerializeField] private List<MindClutter> clutters;
        [SerializeField] private int dirtyCluttersGenerateAmount;

        private bool therapyEnded = false; 

        private void Awake()
        {
            RandomizeClutters();
        }

        private void RandomizeClutters()
        {
            var randomNumbers = GetRandomNumbersInRange(dirtyCluttersGenerateAmount, clutters.Count);
            for (int i = 0; i < clutters.Count; i++) {
                if (randomNumbers.Contains(i)) {
                    clutters[i].SetStateDirty();
                } 
                else {
                    clutters[i].SetStateClean();
                }
                clutters[i].onInteracted += OnClutterInteract;
                clutters[i].onStateChange += OnClutterStateUpdate;
            }
        }

        private void OnDisable() {
            clutters.ForEach(c => 
            {
                c.onInteracted -= OnClutterInteract;
                c.onStateChange -= OnClutterStateUpdate;
            });
        }

        // Utils
        private List<int> GetRandomNumbersInRange(int size, int range)
        {
            return Enumerable.Range(0, range)
                .OrderBy(_ => System.Guid.NewGuid()) // randomize list sort
                .Take(size)
                .ToList();
        }

        private void OnClutterInteract()
        {
            var currentNpc = GameStateController.Instance.SelectedNpc;
            if (therapyEnded) return;

            if (currentNpc.IndoctrinationValue >= 100)
            {
                therapyEnded = true;
                TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Cultist, MoodEnum.Neutral);
                SfxController.Instance.PlaySfx(SfxEnum.IndoctrinationSuccess);


                // Tutorial variables
                TutorialStateVariables.Instance.CanShowFirstSuccessfulRecruit = true;
                TutorialStateVariables.Instance.CanShowFirstTimeSuccessfulTherapy = false;
                TutorialStateVariables.Instance.CanShowFirstTimeFailedTherapy = false;
                
            } 
            else if(!clutters.Any(c => c.CurrentState == ClutterStateEnum.Dirty))
            {
                therapyEnded = true;
                TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Townspeople, MoodEnum.Happy);
                SfxController.Instance.PlaySfx(SfxEnum.TherapySuccess);

                // Tutorial variables
                TutorialStateVariables.Instance.CanShowFirstSuccessfulRecruit = false;
                TutorialStateVariables.Instance.CanShowFirstTimeSuccessfulTherapy = true;
                TutorialStateVariables.Instance.CanShowFirstTimeFailedTherapy = false;
            }
            else if (currentNpc.PatienceValue <= 0)
            {
                therapyEnded = true;
                TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Townspeople, MoodEnum.Angry);
                SfxController.Instance.PlaySfx(SfxEnum.TherapyFail);

                // Tutorial variables
                TutorialStateVariables.Instance.CanShowFirstSuccessfulRecruit = false;
                TutorialStateVariables.Instance.CanShowFirstTimeSuccessfulTherapy = false;
                TutorialStateVariables.Instance.CanShowFirstTimeFailedTherapy = true;
            }

        }

        private void OnClutterStateUpdate()
        {
            var currentNpc = GameStateController.Instance.SelectedNpc;
            if(!clutters.Any(c => c.CurrentState == ClutterStateEnum.Dirty) && currentNpc.IndoctrinationValue < 100)
            {
                therapyEnded = true;
                TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Townspeople, MoodEnum.Happy);
                SfxController.Instance.PlaySfx(SfxEnum.TherapySuccess);

                // Tutorial variables
                TutorialStateVariables.Instance.CanShowFirstSuccessfulRecruit = false;
                TutorialStateVariables.Instance.CanShowFirstTimeSuccessfulTherapy = true;
                TutorialStateVariables.Instance.CanShowFirstTimeFailedTherapy = false;
            }
        }
    }
}