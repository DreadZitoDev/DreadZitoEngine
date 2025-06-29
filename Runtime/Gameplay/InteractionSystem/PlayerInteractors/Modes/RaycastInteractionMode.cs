using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.PlayerInteractors.Modes
{
    public class RaycastInteractionMode : MonoBehaviour, InteractionMode
    {
        [SerializeField] private LayerMask interactorFilter;
        [SerializeField] private float interactableDistance = 2f;
      
        private Camera mainCam;
        
        public Hotspot DetectHotspot()
        {
            mainCam ??= Camera.main;
            if (mainCam == null)
            {
                Debug.LogWarning("Main camera not found. Ensure there is a camera tagged as 'MainCamera'.");
                return null;
            }
            
            var origin = mainCam.transform.position;
            var direction = mainCam.transform.forward;
            var raycast = Physics.Raycast(origin, direction, out var hit, interactableDistance, interactorFilter);
            var hotspot = hit.collider?.GetComponent<Hotspot>();

            hotspot = raycast && hotspot != null && hotspot.IsOn() ? hotspot : null;

            return hotspot;
        }

        public void ExitMode()
        {
            
        }
    }
}