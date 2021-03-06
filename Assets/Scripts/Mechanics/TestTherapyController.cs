namespace Horticultist.Scripts.Mechanics
{
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TestTherapyController : MonoBehaviour
    {
        public void ConvertToHappy(NpcController npc)
        {
            npc.SetPatience(Random.Range(10, 90));
            npc.SetIndoctrination(Random.Range(10, 90));
            npc.ChangeType(NpcTypeEnum.Townspeople);
            npc.SetMood(MoodEnum.Happy);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }
        
        public void ConvertToAngry(NpcController npc)
        {
            npc.SetPatience(0);
            npc.SetIndoctrination(Random.Range(10, 90));
            npc.ChangeType(NpcTypeEnum.Townspeople);
            npc.SetMood(MoodEnum.Angry);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }

        public void ConvertToCultist(NpcController npc)
        {
            npc.SetPatience(Random.Range(10, 90));
            npc.SetIndoctrination(100);
            npc.ChangeType(NpcTypeEnum.Cultist);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }

        public void DecreaseObedience(NpcController npc)
        {
            npc.DecreaseObedienceValue(1);
        }

        public void IncreaseObedience(NpcController npc)
        {
            npc.IncreaseObedienceValue(1);
        }

    }
}