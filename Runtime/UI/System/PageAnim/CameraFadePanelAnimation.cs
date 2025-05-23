using System;
using System.Collections;
using _Room502.Scripts;
using UnityEngine;

namespace DreadZitoEngine.Runtime.UI.System.PageAnim
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CameraFadePanelAnimation : PanelAnimation
    {
        [SerializeField] private float fadeDurationEnter = 1f;
        [SerializeField] private float fadeDurationExit = .6f;
        
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
            fadeCoroutine = StartCoroutine(EnterRoutine());
        }

        public override void Exit(Panel panel, Action onEnd = null)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(ExitRoutine());
        }

        private IEnumerator ExitRoutine()
        {
            yield return Game.Instance.CameraFade.FadeInCameraRoutine(fadeDurationEnter);
            canvasGroup.alpha = 0;
            yield return Game.Instance.CameraFade.FadeOutCameraRoutine(fadeDurationEnter);
            OnEnterAnimationEnd?.Invoke();
        }

        IEnumerator EnterRoutine()
        {
            yield return Game.Instance.CameraFade.FadeInCameraRoutine(fadeDurationExit);
            canvasGroup.alpha = 1;
            yield return Game.Instance.CameraFade.FadeOutCameraRoutine(fadeDurationExit);
            OnExitAnimationEnd?.Invoke();
        }
    }
}