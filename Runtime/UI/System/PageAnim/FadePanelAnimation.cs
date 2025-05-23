using System;
using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.UI.System.PageAnim
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadePanelAnimation : PanelAnimation
    {
        [SerializeField] private float fadeDuration = 0.5f;
        
        private Coroutine fadeCoroutine;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void Enter(Panel panel, Action onEnd = null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(1, () =>
            {
                OnEnterAnimationEnd?.Invoke();
                onEnd?.Invoke();
            }));
        }

        public override void Exit(Panel panel, Action onEnd = null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(Fade(0, () =>
            {
                OnExitAnimationEnd?.Invoke();
                onEnd?.Invoke();
            }));
        }
        
        IEnumerator Fade(float targetValue, Action onEnd = null)
        {
            var t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetValue, t / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = targetValue;
            onEnd?.Invoke();
        }
    }
}