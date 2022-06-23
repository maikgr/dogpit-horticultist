namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Horticultist.Scripts.Extensions;
    using Horticultist.Scripts.UI;

    public class StartScreenController : MonoBehaviour
    {
        [SerializeField] private string nextSceneName;
        [SerializeField] private NpcFactory npcFactory;
        [SerializeField] private SplashUIController splashUIController;
        [SerializeField] private float npcAmount;
        private List<NpcController> generatedNpcs;
        private Camera mainCamera;
        private NpcController trackedNpc;

        private void Awake() {
            mainCamera = Camera.main;
        }

        private void Start() {
            generatedNpcs = new List<NpcController>();
            for (var i = 0; i < npcAmount; ++i)
            {
                generatedNpcs.Add(npcFactory.GenerateNpc());
            }
            trackedNpc = generatedNpcs.GetRandom();

            splashUIController.StartSplash();
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
            splashUIController.FadeOutScene(() => {
                StartCoroutine(NextSceneAsync(nextSceneName));
            });
        }

        private IEnumerator NextSceneAsync(string sceneName)
        {
            var scene = SceneManager.LoadSceneAsync(sceneName);
            while (!scene.isDone)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnDisable() {
            generatedNpcs.ForEach(npc => {
                if(npc != null)
                    Destroy(npc.gameObject);
            });
        }
    }
}