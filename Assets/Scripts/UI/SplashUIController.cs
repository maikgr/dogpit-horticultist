namespace Horticultist.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class SplashUIController : MonoBehaviour
    {
        [SerializeField] private Image fadeScreen;
        [SerializeField] private Image logoScreen;
        [SerializeField] private float fadeTime;
        [SerializeField] private float fadeDelay;
        [SerializeField] private AudioSource bgmSource;

        public void StartSplash()
        {
            DOTween.Sequence()
                .Append(FadeInScreen())
                .AppendInterval(fadeDelay)
                .Append(FadeOutScreen().OnComplete(() => logoScreen.gameObject.SetActive(false)))
                .Append(FadeInScreen())
                .OnComplete(() => fadeScreen.gameObject.SetActive(false))
                .SetId("splash");
        }

        private void OnDisable() {
            DOTween.Kill("splash");
        }

        public void FadeOutScene(System.Action action)
        {
            fadeScreen.gameObject.SetActive(true);
            DOTween.Sequence()
                .Append(FadeOutScreen())
                .Join(bgmSource.DOFade(0, fadeTime))
                .OnComplete(() => action.Invoke())
                .SetId("splash");
        }

        private Tween FadeInScreen()
        {
            return DOVirtual.Float(1, 0, fadeTime, (val) =>
            {
                fadeScreen.color = new Color(
                    fadeScreen.color.r,
                    fadeScreen.color.g,
                    fadeScreen.color.b,
                    val
                );
            })
            .SetEase(Ease.Linear);
        }

        private Tween FadeOutScreen()
        {
            fadeScreen.gameObject.SetActive(true);
            return DOVirtual.Float(0, 1, fadeTime, (val) =>
            {
                fadeScreen.color = new Color(
                    fadeScreen.color.r,
                    fadeScreen.color.g,
                    fadeScreen.color.b,
                    val
                );
            })
            .SetEase(Ease.Linear);
        }
    }

}