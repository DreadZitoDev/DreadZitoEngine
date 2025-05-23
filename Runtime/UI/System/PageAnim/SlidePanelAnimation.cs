using System;
using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.UI.System.PageAnim
{
    public class SlidePanelAnimation : PanelAnimation
    {
        [SerializeField] private AnimationCurve enterCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve exitCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [SerializeField] private float duration = 1f;
        [SerializeField] private Vector3 enterPosition;
        [SerializeField] private Vector3 exitPosition;

        private Coroutine routine;
        
        public override void Enter(Panel panel, Action onEnd = null)
        {
            panel.CanvasGroup.alpha = 1;
            if (routine != null)
            {
                StopCoroutine(routine);
            }
            Vector3 startPos = panel.GetComponent<RectTransform>().anchoredPosition;
            routine = StartCoroutine(Animate(startPos, enterPosition, panel, enterCurve, () =>
            {
                OnEnterAnimationEnd?.Invoke();
                onEnd?.Invoke();
            }));
        }

        public override void Exit(Panel panel, Action onEnd = null)
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }
            Vector3 startPos = panel.GetComponent<RectTransform>().anchoredPosition;
            routine = StartCoroutine(Animate(startPos, exitPosition, panel, exitCurve, () =>
            {
                OnExitAnimationEnd?.Invoke();
                onEnd?.Invoke();
            }));
        }

        IEnumerator Animate(Vector3 startPos, Vector3 endPos, Panel panel, AnimationCurve curve, Action onEnd = null)
        {
            float time = 0;

            var rectTransform = panel.GetComponent<RectTransform>();
            
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = curve.Evaluate(time / duration);
                rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }

            onEnd?.Invoke();
        }
    }
}