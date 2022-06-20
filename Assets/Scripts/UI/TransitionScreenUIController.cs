namespace Horticultist.Scripts.UI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class TransitionScreenUIController : MonoBehaviour
    {
        [SerializeField] private Image transScreen;
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private float transTime;
        [SerializeField] private float baseTransScreenSize;

        private void Start() {
            StartCoroutine(DelayedStart());
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(1f);
            TransitionOut();
        }

        public void TransitionIn(System.Action action = null)
        {
            TransitionIn(Vector2.zero, action);
        }

        public void TransitionIn(Vector2 target, System.Action action = null)
        {
            SfxController.Instance.PlaySfx(Core.SfxEnum.TransitionIn);
            DOTween.Sequence()
                .Append(
                    transScreen.rectTransform.DOMove(target, 0)
                )
                .Join(
                    DOVirtual.Float(baseTransScreenSize, 1, transTime, (val) =>
                    {
                        transScreen.rectTransform.sizeDelta = new Vector2(val, val);
                    })
                )
                .Join(
                    bgmSource.DOFade(0, transTime)
                )
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                });
        }

        public void TransitionOut(System.Action action = null)
        {
            TransitionOut(Vector2.zero, action);
        }

        public void TransitionOut(Vector2 target, System.Action action = null)
        {
            bgmSource.volume = 0;
            SfxController.Instance.PlaySfx(Core.SfxEnum.TransitionOut);
            DOTween.Sequence()
                .Append(
                    transScreen.rectTransform.DOMove(target, 0)
                )
                .Join(
                    DOVirtual.Float(1, baseTransScreenSize, transTime, (val) =>
                    {
                        transScreen.rectTransform.sizeDelta = new Vector2(val, val);
                    })
                )
                .Join(
                    bgmSource.DOFade(1, transTime)
                )
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    if (action != null)
                    {
                        action.Invoke();
                    }
                });
        }
    }
}
