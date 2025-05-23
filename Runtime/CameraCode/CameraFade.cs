using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace DreadZitoEngine.Runtime.CameraCode
{
    public enum FadeCameraMode
    {
        FadeIn,
        FadeOut
    }
    
    public enum FadeMethod
    {
        OnGUI,
        PostProcess
    }
    
    public class CameraFade : MonoBehaviour
    {
        public Volume volume;
        public Color fadeColor = Color.black;
        // Rather than Lerp or Slerp, we allow adaptability with a configurable curve
        public AnimationCurve Curve = AnimationCurve.Linear(0, 0, 1, 1);

        private Texture2D texture;
        private Texture2D Texture => texture ??= SetupTexture();
        private float alpha;
        
        public bool IsFadedIn  => alpha >= 1;
        
        private FadeMethod fadeMethod = FadeMethod.OnGUI;
        
        private Texture2D SetupTexture()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0));
            texture.Apply();
            return texture;
        }
        
        bool showTexture = false;
        public void OnGUI()
        {
            if (showTexture)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture);
        }

        private Coroutine fadeRoutine;
        
        public void FadeOut(float duration, Action onFadeComplete = null, FadeMethod method = FadeMethod.OnGUI)
        {
            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }

            Debug.Log($"Fading out camera with duration: {duration}, target alpha: {0f}");
            fadeRoutine = StartCoroutine(FadeCoroutine(0f, duration, () =>
            {
                onFadeComplete?.Invoke();
                showTexture = false;
            }, method));
        }

        public IEnumerator FadeOutCameraRoutine(float duration = 1f, Action onFadeComplete = null,  FadeMethod method = FadeMethod.OnGUI)
        {
            FadeOut(duration, onFadeComplete, method);
            yield return new WaitForSeconds(duration);
        }
        
        public void FadeIn(float duration, Action onFadeComplete = null, FadeMethod method = FadeMethod.OnGUI)
        {
            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }
            fadeRoutine = StartCoroutine(FadeCoroutine(1f, duration, onFadeComplete, method));
        }
        
        public IEnumerator FadeInCameraRoutine(float duration = 1f, Action onFadeComplete = null, FadeMethod method = FadeMethod.OnGUI)
        {
            if (IsFadedIn)
            {
                if (method == fadeMethod)
                    yield break;
                // If the method is different switch methods
                DetectMethodChanges(method, 1);
            }
            FadeIn(duration, onFadeComplete, method);
            yield return new WaitForSeconds(duration);
        }

        public void SetFade(float alpha, bool useOnGui = true)
        {
            if (useOnGui)
                SetFadeOnGui(alpha);
            else
                SetFadePostProcess(alpha);
        }
        
        private void SetFadeOnGui(float alpha)
        {
            this.alpha = alpha;
            Texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            Texture.Apply();
            
            showTexture = alpha > 0;
        }
        
        private void SetFadePostProcess(float alpha)
        {
            this.alpha = alpha;
            volume.weight = alpha;
        }
        
        IEnumerator FadeCoroutine(float targetAlpha, float duration, Action onFadeComplete, FadeMethod method)
        {
            DetectMethodChanges(method);
            if (method == FadeMethod.OnGUI)
                yield return FadeCoroutineOnGui(targetAlpha, duration, onFadeComplete);
            else
                yield return FadeCoroutinePostProcess(targetAlpha, duration, onFadeComplete);
        }
        
        private IEnumerator FadeCoroutineOnGui(float targetAlpha, float duration, Action onFadeComplete)
        {
            var startAlpha = Texture.GetPixel(0, 0).a;
            var time = 0f;
            
            while (time < duration)
            {
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, Curve.Evaluate(time / duration));
                SetFadeOnGui(alpha);
                time += Time.deltaTime;
                yield return null;
            }
            SetFadeOnGui(targetAlpha);
            fadeRoutine = null;
            
            onFadeComplete?.Invoke();
        }
        
        private IEnumerator FadeCoroutinePostProcess(float targetAlpha, float duration, Action onFadeComplete)
        {
            var startAlpha = volume.weight;
            var time = 0f;
            
            while (time < duration)
            {
                var alpha = Mathf.Lerp(startAlpha, targetAlpha, Curve.Evaluate(time / duration));
                SetFadePostProcess(alpha);
                time += Time.deltaTime;
                yield return null;
            }
            SetFadePostProcess(targetAlpha);
            fadeRoutine = null;
            
            onFadeComplete?.Invoke();
        }

        /// <summary>
        /// Detects if the fade method has changed and sets the fade accordingly.
        /// </summary>
        private void DetectMethodChanges(FadeMethod method, float targetAlpha = -1)
        {
            Debug.Log($"[DB] Checking fade method: {method}, current: {fadeMethod}");
            if (fadeMethod == method) return;
            var oldMethod = fadeMethod;
            if (oldMethod == FadeMethod.OnGUI)
            {
                SetFadeOnGui(0);
            }
            else if (oldMethod == FadeMethod.PostProcess)
            {
                SetFadePostProcess(0);
            }
            
            fadeMethod = method;
            if (targetAlpha >= 0)
            {
                if (fadeMethod == FadeMethod.OnGUI)
                    SetFadeOnGui(targetAlpha);
                else
                    SetFadePostProcess(targetAlpha);
            }
        }
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(CameraFade))]
    public class CameraFadeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var cameraFade = (CameraFade) target;
            if (GUILayout.Button("Fade Out"))
            {
                cameraFade.FadeOut(1f);
            }
            if (GUILayout.Button("Fade In"))
            {
                cameraFade.FadeIn(1f);
            }
        }
    }
#endif
}