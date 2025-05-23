using DreadZitoEngine.Runtime.Gameplay.InteractionSystem;
using DreadZitoEngine.Runtime.UI.System;
using UnityEngine;

namespace DreadZitoEngine.Runtime.UI.Gameplay
{
    public class HotspotUI : MonoBehaviour
    {
        private Panel hotspotNamePanel;

        //[Header("Hotspot Name UI")]
        //[SerializeField] private TextMeshProUGUI hotspotNameText;
        
        private Canvas canvas;

        private Coroutine displayInteractionUIRoutine;
        private Coroutine positionNameHotspotElementRoutine;
        
        public void Init(Canvas canvas, Panel hotspotNamePanel)
        {
            this.canvas = canvas;
            this.hotspotNamePanel = hotspotNamePanel;
        }

        public void ShowHotspotName(Interactable hotspot)
        {
            //hotspotNameText.text = hotspot.InteractableName;
        }
        
        public void ShowHotspotName(Hotspot hotspot)
        {
            //hotspotNameText.text = hotspot.HotspotName;
        }
    }
}