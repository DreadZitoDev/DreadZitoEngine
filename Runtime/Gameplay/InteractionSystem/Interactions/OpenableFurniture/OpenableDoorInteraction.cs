using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions.OpenableFurniture
{
    public class OpenableDoorInteraction : OpenableFurnitureInteraction
    {
        [Space]
        [SerializeField] private Vector3 openEulerRotation;
        [SerializeField] private Vector3 closeEulerRotation;
        
        [SerializeField] private AnimationCurve openCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private AnimationCurve closeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        private Quaternion openTargetRotation;
        private Quaternion closeTargetRotation;

        [Header("Handle")]
        [SerializeField] private Transform handleTransform;
        [SerializeField] private AnimationCurve handleCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float handleRotation = 45f;
        private quaternion handleStartRotation;
        private quaternion handleMaxRoatation;
        
        private void Awake()
        {
            base.Awake(); // Sets runtimeIsOpen based on startOpen

            openTargetRotation = Quaternion.Euler(openEulerRotation);
            closeTargetRotation = Quaternion.Euler(closeEulerRotation);
            
            // Initialize door rotation based on start state
            if (runtimeIsOpen)
                furnitureTransform.localRotation = openTargetRotation;
            else
                furnitureTransform.localRotation = closeTargetRotation;
            
            if (handleTransform == null) return;
            handleStartRotation = handleTransform.localRotation;
            handleMaxRoatation = quaternion.Euler(0, 0, handleRotation);
        }
        
        public override IEnumerator OpenRoutine(Hotspot hotspot)
        {
            yield return LerpRoutine(openTargetRotation, openCurve, openDuration);
            runtimeIsOpen = true; // Update state after opening
        }
        
        public override IEnumerator CloseRoutine(Hotspot hotspot)
        {
            yield return LerpRoutine(closeTargetRotation, closeCurve, openDuration);
            runtimeIsOpen = false; // Update state after closing
        }

        private IEnumerator LerpRoutine(Quaternion targetRot, AnimationCurve curve, float duration)
        {
            Quaternion startRotation = furnitureTransform.localRotation;
            float elapsedTime = 0f;

            while (elapsedTime < openDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / openDuration);
                var progress = curve.Evaluate(t);
                furnitureTransform.localRotation = Quaternion.Lerp(startRotation, targetRot, progress);
                
                if (handleTransform != null)
                {
                    handleTransform.localRotation = Quaternion.Lerp(handleStartRotation, handleMaxRoatation, handleCurve.Evaluate(t));
                }
                
                yield return null;
            }
            furnitureTransform.localRotation = targetRot;
        }

        public void SetOpen(bool open)
        {
            var routine = LerpRoutine(open ? openTargetRotation : closeTargetRotation, open ? openCurve : closeCurve, 0);
            while (routine.MoveNext()) {
            }
            runtimeIsOpen = open;
        }
    }
}