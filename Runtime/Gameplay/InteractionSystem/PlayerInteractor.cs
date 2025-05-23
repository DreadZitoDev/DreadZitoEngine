using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private LayerMask interactorFilter;
        [SerializeField] private float interactableDistance = 2f;
        
        private Interactable currentInteractable;
        public Hotspot CurrentHotspot { get; private set; }
        
        private Camera mainCam;

        private Coroutine executingInteraction;
        
        public void Init()
        {
            mainCam = Camera.main;
        }
        
        private void Update()
        {
            CheckInteractables();
        }

        private void CheckInteractables()
        {
            var origin = mainCam.transform.position;
            var direction = mainCam.transform.forward;
            var raycast = Physics.Raycast(origin, direction, out var hit, interactableDistance, interactorFilter);
            var hotspot = hit.collider?.GetComponent<Hotspot>();

            if (raycast && hotspot != null && hotspot.IsOn())
            {
                CurrentHotspot = hotspot;
            }
            else
            {
                CurrentHotspot = null;
            }
        }
        
        public void SetCanInteract(bool value)
        {
            enabled = value;
            if (!value)
                CurrentHotspot = null;
        }
    }
}