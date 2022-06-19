namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Horticultist.Scripts.Extensions;

    public class StartScreenController : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        [SerializeField] private NpcFactory npcFactory;
        private Camera mainCamera;
        private NpcController trackedNpc;

        private void Awake() {
            mainCamera = Camera.main;
        }

        private void Start() {
            var npcList = new List<NpcController>();
            for (var i = 0; i < 5; ++i)
            {
                npcList.Add(npcFactory.GenerateNpc());
            }
            trackedNpc = npcList.GetRandom();
        }

        private void LateUpdate() {
            mainCamera.transform.position = new Vector3(
                trackedNpc.transform.position.x,
                trackedNpc.transform.position.y,
                mainCamera.transform.position.z
            );
        }

        public void OnStartClick()
        {
            StartCoroutine(NextSceneAsync(nextSceneName));
        }

        private IEnumerator NextSceneAsync(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);
            while (!scene.isDone)
            {
                yield return new WaitForFixedUpdate();
            }
        }
    }
}