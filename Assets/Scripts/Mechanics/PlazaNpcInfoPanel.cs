namespace Horticultist.Scripts.Mechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using DG.Tweening;

    [RequireComponent(typeof(NpcController))]
    public class PlazaNpcInfoPanel : MonoBehaviour
    {
        public void StartTherapy()
        {
            GameStateController.Instance.SetSelectedNpc(GetComponent<NpcController>());
            DOTween.Pause("npc");
            StartCoroutine(LoadTherapyScene());
        }

        private IEnumerator LoadTherapyScene()
        {
            var asyncLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}
