using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.PlayerInteractors.Modes
{
    public class HardSetInteractionMode : MonoBehaviour, InteractionMode
    {
        private Hotspot currentHotspot;
        
        public Hotspot DetectHotspot()
        {
            if (currentHotspot == null || !currentHotspot.IsOn())
            {
                Debug.LogWarning("No valid hotspot set or the current hotspot is turned off.");
                return null;
            }
            return currentHotspot;
        }

        public void SetHotspot(Hotspot hotspot)
        {
            if (hotspot == null)
            {
                Debug.LogWarning("Attempted to set a null hotspot.");
                return;
            }
            currentHotspot = hotspot;
        }
        
        public void ClearHotspot()
        {
            currentHotspot = null;
        }
        
        public void ExitMode()
        {
            ClearHotspot();
        }
    }
}