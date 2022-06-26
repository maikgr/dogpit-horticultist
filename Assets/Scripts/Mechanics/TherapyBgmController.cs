namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class TherapyBgmController : MonoBehaviour
    {
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private List<TherapyBgmMap> bgmMaps;

        private void Start()
        {
            var personality = GameStateController.Instance.SelectedNpc.npcPersonality;
            var bgm = bgmMaps.First(map => map.personality == personality).audioBgm;
            bgmSource.clip = bgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    [System.Serializable]
    public class TherapyBgmMap
    {
        public NpcPersonalityEnum personality;
        public AudioClip audioBgm;
    }
}