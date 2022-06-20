namespace Horticultist.Scripts.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;


    public class FadeUIController : MonoBehaviour
    {
        [SerializeField] private Image fadeScreen;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private float fadeTime;

        public void FadeInScreen(Action onComplete)
        {
            // Screen
            fadeScreen.gameObject.SetActive(true);
            DOVirtual.Float(1, 0, fadeTime, (val) =>
            {
                fadeScreen.color = new Color(
                    fadeScreen.color.r,
                    fadeScreen.color.g,
                    fadeScreen.color.b,
                    val
                );
            })
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                fadeScreen.gameObject.SetActive(false);
                onComplete.Invoke();
            });

            // BGM
            bgmSource.volume = 0;
            bgmSource.DOFade(1, fadeTime);
        }

        public void FadeOutScreen(Action onComplete)
        {
            // Screen
            fadeScreen.gameObject.SetActive(true);
            DOVirtual.Float(0, 1, fadeTime, (val) =>
            {
                fadeScreen.color = new Color(
                    fadeScreen.color.r,
                    fadeScreen.color.g,
                    fadeScreen.color.b,
                    val
                );
            })
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                onComplete.Invoke();
            });

            // BGM
            bgmSource.DOFade(0, fadeTime);
        }
    }
}
