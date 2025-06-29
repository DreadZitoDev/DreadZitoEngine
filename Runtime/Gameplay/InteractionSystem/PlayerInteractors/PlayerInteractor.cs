using DreadZitoEngine.Runtime.Gameplay.InteractionSystem.PlayerInteractors.Modes;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public enum Mode
    {
        Raycast,
        HardSet,
    }
    
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private LayerMask interactorFilter;
        [SerializeField] private float interactableDistance = 2f;

        public Hotspot CurrentHotspot { get; private set; }
        
        [SerializeField] private Mode defaultMode = Mode.Raycast;
        private Mode mode;
        
        private InteractionMode currentMode;
        
        private RaycastInteractionMode raycastMode;
        private HardSetInteractionMode hardSetMode;
        
        public void Init()
        {
            raycastMode = GetComponentInChildren<RaycastInteractionMode>();
            hardSetMode = GetComponentInChildren<HardSetInteractionMode>();
            SwitchMode(defaultMode);
        }
        
        private void Update()
        {
            //CheckInteractables();
            CurrentHotspot = currentMode.DetectHotspot();
        }
        
        public void SetCanInteract(bool value)
        {
            enabled = value;
            if (!value)
                CurrentHotspot = null;
        }
        
        public void SwitchMode(Mode newMode)
        {
            mode = newMode;
            currentMode?.ExitMode();
            currentMode = newMode switch
            {
                Mode.Raycast => raycastMode,
                Mode.HardSet => hardSetMode,
                _ => throw new System.ArgumentOutOfRangeException(nameof(newMode), newMode, null)
            };
        }

        public void SelectHotspot(Hotspot runtimeSubHotspot)
        {
            if (mode != Mode.HardSet)
            {
                Debug.LogWarning("SelectHotspot can only be used in HardSet mode.");
                return;
            }
            
            hardSetMode.SetHotspot(runtimeSubHotspot);
        }

        public void ClearHotspot()
        {
            if (mode != Mode.HardSet)
            {
                Debug.LogWarning("ClearHotspot can only be used in HardSet mode.");
                return;
            }

            hardSetMode.ClearHotspot();
        }
    }
}