using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace DreadZitoEngine.Runtime.Tools
{
    [RequireComponent(typeof(Volume))]
    public class VolumeLerper : MonoBehaviour
    {
        [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1);

        private Volume volume;

        private void Awake()
        {
            volume ??= GetComponent<Volume>();
        }

        public void StartWeightLerp(float duration)
        {
            StartCoroutine(Lerp(0, 1, duration));
        }
        
        IEnumerator Lerp(float startValue, float endValue, float duration)
        {
            float time = 0;
            while (time < duration)
            {
                var t = time / duration;
                var evaluate = lerpCurve.Evaluate(t);
                SetValue(Mathf.Lerp(startValue, endValue, evaluate));
                
                time += Time.deltaTime;
                yield return null;
            }
            
            SetValue(endValue);
        }

        public void SetValue(float value)
        {
            volume.weight = value;
        }
    }
}