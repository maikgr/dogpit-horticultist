namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TestTherapyController : MonoBehaviour
    {
        public void ConvertToHappy(NpcController npc)
        {
            npc.SetPatience(Random.Range(10, 90));
            npc.SetIndoctrination(Random.Range(10, 90));
            npc.ChangeType(NpcTypeEnum.HappyTownspeople);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }
        
        public void ConvertToAngry(NpcController npc)
        {
            npc.SetPatience(0);
            npc.SetIndoctrination(Random.Range(10, 90));
            npc.ChangeType(NpcTypeEnum.AngryTownspeople);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }

        public void ConvertToCultist(NpcController npc)
        {
            npc.SetPatience(Random.Range(10, 90));
            npc.SetIndoctrination(100);
            npc.ChangeType(NpcTypeEnum.Cultist);
            Debug.Log("updated " + npc.DisplayName + " to " + npc.NpcType.ToString());
        }
    }
}