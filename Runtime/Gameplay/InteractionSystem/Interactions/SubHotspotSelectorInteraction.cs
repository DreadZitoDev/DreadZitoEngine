using System.Collections;
using System.Collections.Generic;
using DreadZitoEngine.Runtime.Inputs;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class SubHotspotSelectorInteraction : HotspotInteractionBase
    {
        [SerializeField] private List<Hotspot> subHotspots = new List<Hotspot>();
        [SerializeField] private HotspotInteractionBase triggerSelectionInteraction;
        [SerializeField] private HotspotInteractionBase leaveInteraction;
        
        private List<Hotspot> runtimeSubHotspots = new List<Hotspot>();
        
        private int currentIndex;

        private bool initialized = false;
        
        PlayerInteractor playerInteractor => GameplayMain.Instance.Player.Interactor;

        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            if (!initialized)
                Initialize();

            StartInteraction();
            
            var navigationInput = InputBridge.UI.Navigate;
            var cancelInput = InputBridge.UI.Cancel;

            if (runtimeSubHotspots.Count > 0)
            {
                playerInteractor.SelectHotspot(runtimeSubHotspots[currentIndex]);
            }
            
            while (true)
            {
                var horizontal = navigationInput.WasPressedThisFrame() ? navigationInput.ReadValue<Vector2>().x : 0;
                var cancelButtonPressed = cancelInput.WasPressedThisFrame();
                var noMoreHotspots = runtimeSubHotspots.Count == 0;
                
                if (cancelButtonPressed || noMoreHotspots)
                {
                    if (noMoreHotspots)
                        this.TurnOff();
                    else
                        playerInteractor.ClearHotspot(); 
                    yield return leaveInteraction.ExecuteRoutine(hotspot);
                    EndInteraction();
                    yield break;
                }
                
                if (horizontal > 0)
                {
                    currentIndex--;
                    if (currentIndex < 0)
                        currentIndex = runtimeSubHotspots.Count - 1;
                    playerInteractor.SelectHotspot(runtimeSubHotspots[currentIndex]);
                }
                else if (horizontal < 0)
                {
                    currentIndex++;
                    if (currentIndex >= runtimeSubHotspots.Count)
                        currentIndex = 0;
                    playerInteractor.SelectHotspot(runtimeSubHotspots[currentIndex]);
                }
                
                yield return null;
            }
        }

        private void StartInteraction()
        {
            playerInteractor.SwitchMode(Mode.HardSet);
        }
        
        private void EndInteraction()
        {
            playerInteractor.SwitchMode(Mode.Raycast);
        }

        private void Initialize()
        {
            runtimeSubHotspots = new List<Hotspot>(subHotspots);
            
            var interactionSystem = GameplayMain.Instance.InteractionSystemHandler;
            //interactionSystem.OnHotspotInteraction += OnHotspotInteraction;
        }
    }
}