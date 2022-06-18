namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Random=System.Random;

    public class RoomController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> clutters;
        [SerializeField] private int numberOfClutters;

        private void Awake()
        {
            RandomizeClutters();
        }

        private void RandomizeClutters()
        {
            var randomNumbers = GetRandomNumbersInRange(numberOfClutters, clutters.Count);
            for (int i = 0; i < clutters.Count; i++) {
                if (randomNumbers.Contains(i)) {
                    clutters[i].GetComponent<MindClutter>().SetStateDirty();
                } 
                else {
                    clutters[i].GetComponent<MindClutter>().SetStateClean();
                }
            }
        }

        // Utils
        private List<int> GetRandomNumbersInRange(int size, int range)
        {
            // if (number > range) throw error

            List<int> numbers = new List<int>();
            int number = 0;
            Random r = new Random();

            for (int i = 0; i < size; i++) {
                number = r.Next(0, range);
                while (numbers.Contains(number)) {
                    number = r.Next(0, range);
                }
                numbers.Add(number);
            }

            return numbers;
        }
    }
}