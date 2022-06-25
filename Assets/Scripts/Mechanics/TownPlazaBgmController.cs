namespace Horticultist.Scripts.Mechanics
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class TownPlazaBgmController : MonoBehaviour
    {
        public static TownPlazaBgmController Instance { get; private set; }
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private List<TreeBgmMap> bgmMap;

        private void Awake() {
            var instances = GameObject.FindObjectsOfType<TownPlazaBgmController>();
            if (instances.Length > 1)
            {
                Debug.LogError("Only one TownPlazaBgmController allowed");
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Start() {
            var stage = GameStateController.Instance.TreeStage;
            var stageBgm = bgmMap.First(b => b.TreeStage.Equals(stage));

            bgmSource.clip = stageBgm.bgm;
            bgmSource.Play();
            bgmSource.loop = true;
        }

        private void OnDisable() {
            if (bgmCoroutine != null)
            {
                StopCoroutine(bgmCoroutine);
            }
        }

        public void PauseBgm()
        {
            bgmSource.Pause();
        }

        private Coroutine bgmCoroutine;
        public void ResumeBgm(float delay = 0)
        {
            bgmCoroutine = StartCoroutine(DelayedResume(delay));
        }

        private IEnumerator DelayedResume(float delay)
        {
            yield return new WaitForSeconds(delay);
            bgmSource.UnPause();
        }
        
    }

    [System.Serializable]
    public class TreeBgmMap
    {
        public int TreeStage;
        public AudioClip bgm;
    }
}