using System.Collections.Generic;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Environment
{
    public class Mirror : MonoBehaviour
    {
        public Camera mirrorCamera;
        [SerializeField] private float nearClipOffset = 0.2f;
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] private MeshRenderer mirrorRend;
        
        private Camera mainCamera;

        [SerializeField] private bool MirrorReflectBehaviourLoco = false;

        public List<GameObject> dynamicObjectsInRange = new();
        
        void Start()
        {
            // Assign main camera if not set
            if (!mainCamera) mainCamera = Camera.main;
            mirrorRend ??= GetComponent<MeshRenderer>();
            // Capture the mirror at start
            mirrorCamera.Render();
        }

        void LateUpdate()
        {
            if (!mainCamera || !mirrorCamera) return;

            mirrorCamera.enabled = IsVisibleFrom(mirrorRend, mainCamera) &&
                                   Vector3.Distance(mainCamera.transform.position, transform.position) < maxDistance
                                   && HasAnyDynamicVisible();
            if (!mirrorCamera.enabled || !MirrorReflectBehaviourLoco) return;
            
            Vector3 mirrorPos = transform.position;
            Vector3 mirrorNormal = transform.forward;

            // Calculate reflected position
            Vector3 mainCamPos = mainCamera.transform.position;
            float distance = Vector3.Dot(mainCamPos - mirrorPos, mirrorNormal);
            Vector3 reflectedPos = mainCamPos - 2 * distance * mirrorNormal;
            
            // Apply transformations
            //mirrorCamera.transform.position = reflectedPos;
            mirrorCamera.transform.LookAt(transform.position, transform.up);

            var dist = Vector3.Distance(mirrorCamera.transform.position, transform.position);
            mirrorCamera.nearClipPlane = dist + nearClipOffset;
        }

        private bool HasAnyDynamicVisible()
        {
            foreach (var dynamicObject in dynamicObjectsInRange)
            {
                var rend = dynamicObject?.GetComponentInChildren<Renderer>();
                if (rend && IsVisibleFrom(rend, mirrorCamera))
                {
                    return true;
                }
            }
            
            return false;
        }

        public bool IsVisibleFrom(Renderer renderer, Camera camera)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        private void OnTriggerEnter(Collider other)
        {
            var isDynamyc = !other.gameObject.isStatic;
            if (isDynamyc)
            {
                dynamicObjectsInRange.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var isDynamyc = !other.gameObject.isStatic;
            if (isDynamyc && dynamicObjectsInRange.Contains(other.gameObject))
            {
                dynamicObjectsInRange.Remove(other.gameObject);
            }
        }
    }
}