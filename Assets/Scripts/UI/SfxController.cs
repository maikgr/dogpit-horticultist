namespace Horticultist.Scripts.UI
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Horticultist.Scripts.Core;

    public class SfxController : MonoBehaviour
    {
        public static SfxController Instance { get; private set; }
        [SerializeField] private List<SfxAudioMap> audioMaps;

        private void Awake() {
            var eventBus = GameObject.FindObjectsOfType<SfxController>();
            if (eventBus.Length > 1)
            {
                Debug.LogError("Only one SfxController can be active!");
            }

            Instance = this;
        }

        public void PlaySfx(SfxEnum sfx)
        {
            PlaySfx(sfx, Vector2.zero);
        }
        
        public void PlaySfx(SfxEnum sfx, Vector2 position)
        {
            var audioSfx = audioMaps.First(s => s.sfx.Equals(sfx)).audio;
            var sfObj = new GameObject(sfx.ToString())
                .AddComponent<AudioSource>();
            sfObj.clip = audioSfx;
            sfObj.Play();
            Destroy(sfObj.gameObject, audioSfx.length);
        }
    }

    [Serializable]
    public class SfxAudioMap
    {
        public SfxEnum sfx;
        public AudioClip audio;
    }
}