using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions.OpenableFurniture
{
    public class OpenableDrawerInteractable : OpenableFurnitureInteraction
    {
        [Space]
        [SerializeField] private UnityEvent OnOpen;
        [SerializeField] private UnityEvent OnClose;
        
        [Space]
        [SerializeField] private Vector3 openDrawerPos;
        [SerializeField] private Vector3 closeDrawerPos;
        
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private AnimationCurve closeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private void Awake()
        {
            base.Awake(); // Sets runtimeIsOpen based on startOpen

            // Initialize door rotation based on start state
            if (runtimeIsOpen)
                furnitureTransform.localPosition = openDrawerPos;
            else
                furnitureTransform.localPosition = closeDrawerPos;
        }

        public override IEnumerator OpenRoutine(Hotspot hotspot)
        {
            yield return LerpRoutine(openDrawerPos, openCurve);
            runtimeIsOpen = true; // Update state after opening
        }
        
        public override IEnumerator CloseRoutine(Hotspot hotspot)
        {
            yield return LerpRoutine(closeDrawerPos, closeCurve);
            runtimeIsOpen = false; // Update state after closing
        }

        private IEnumerator LerpRoutine(Vector3 targetPos, AnimationCurve curve)
        {
            var startRotation = furnitureTransform.localPosition;
            float elapsedTime = 0f;

            while (elapsedTime < openDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / openDuration);
                var progress = curve.Evaluate(t);
                furnitureTransform.localPosition = Vector3.Lerp(startRotation, targetPos, progress);
                yield return null;
            }
            furnitureTransform.localPosition = targetPos;
        }
    }
}