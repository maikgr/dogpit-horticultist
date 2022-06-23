namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random=System.Random;
    using Horticultist.Scripts.Core;

    public class RoomController : MonoBehaviour
    {
        [SerializeField] private List<MindClutter> clutters;
        [SerializeField] private int dirtyCluttersGenerateAmount;

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
                clutters[i].onInteracted += OnClutterStateUpdate;
            }
        }

        private void OnDisable() {
            clutters.ForEach(c => c.onInteracted -= OnClutterStateUpdate);
        }

        // Utils
        private List<int> GetRandomNumbersInRange(int size, int range)
        {
            return Enumerable.Range(0, range)
                .OrderBy(_ => System.Guid.NewGuid()) // randomize list sort
                .Take(size)
                .ToList();

            // List<int> numbers = new List<int>();
            // int number = 0;
            // Random r = new Random();

            // for (int i = 0; i < size; i++) {
            //     number = r.Next(0, range);
            //     while (numbers.Contains(number)) {
            //         number = r.Next(0, range);
            //     }
            //     numbers.Add(number);
            // }

            // return numbers;
        }

        private void OnClutterStateUpdate()
        {
            if(!clutters.Any(c => c.CurrentState == ClutterStateEnum.Dirty))
            {
                TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Townspeople, MoodEnum.Happy);
            }
            else
            {
                var currentNpc = GameStateController.Instance.SelectedNpc;
                if (currentNpc.PatienceValue <= 0)
                {
                    TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Townspeople, MoodEnum.Angry);
                }
                else if (currentNpc.IndoctrinationValue >= 100)
                {
                    TherapyEventBus.Instance.DispatchOnTherapyEnds(NpcTypeEnum.Cultist, MoodEnum.Neutral);
                }
            }
        }
    }
}