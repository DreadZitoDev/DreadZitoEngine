using System;
using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions.OpenableFurniture
{
    [Serializable]
    public struct BoxColliderData
    {
        public Vector3 center;
        public Vector3 size;
    }
    
    public class OpenableCourtineInteraction : OpenableFurnitureInteraction
    {
        [Header("Courtine Settings")]
        private SkinnedMeshRenderer courtineRenderer;
        [SerializeField] private AnimationCurve lerpCurve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [SerializeField] private BoxColliderData openedColliderData;
        [SerializeField] private BoxColliderData closedColliderData;

        public override void SetHotspot(Hotspot hotspot)
        {
            base.SetHotspot(hotspot);
            if (runtimeIsOpen)
                SetBoxColliderData(openedColliderData, Hotspot.Collider as BoxCollider);
            else
                SetBoxColliderData(closedColliderData, Hotspot.Collider as BoxCollider);
        }

        public override IEnumerator OpenRoutine(Hotspot hotspot)
        {
            // Set opened collider data
            var hotspotBoxCollider = hotspot.Collider as BoxCollider;
            if (hotspotBoxCollider != null)
            {
                hotspotBoxCollider.center = openedColliderData.center;
                hotspotBoxCollider.size = openedColliderData.size;
            }
            
            yield return LerpOpenValue(0, 100, openDuration);
            runtimeIsOpen = true;
        }
        
        public override IEnumerator CloseRoutine(Hotspot hotspot)
        {
            // Set closed collider data
            var hotspotBoxCollider = hotspot.Collider as BoxCollider;
            if (hotspotBoxCollider != null)
            {
                hotspotBoxCollider.center = closedColliderData.center;
                hotspotBoxCollider.size = closedColliderData.size;
            }
            
            yield return LerpOpenValue(100, 0, openDuration);
            runtimeIsOpen = false;
        }
        
        private IEnumerator LerpOpenValue(float startValue, float endValue, float duration)
        {
            courtineRenderer ??= furnitureTransform.GetComponentInChildren<SkinnedMeshRenderer>();
            
            float time = 0;
            while (time < duration)
            {
                var t = time / duration;
                var evaluate = lerpCurve.Evaluate(t);
                courtineRenderer.SetBlendShapeWeight(0, Mathf.Lerp(startValue, endValue, evaluate));
                
                time += Time.deltaTime;
                yield return null;
            }
            
            courtineRenderer.SetBlendShapeWeight(0, endValue);
        }
        
        private void SetBoxColliderData(BoxColliderData data, BoxCollider collider)
        {
            collider.center = data.center;
            collider.size = data.size;
        }
    }
}