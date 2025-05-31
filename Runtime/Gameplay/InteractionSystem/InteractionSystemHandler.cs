using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.Gameplay.Players;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    [System.Serializable]
    public class InteractionExecutionData
    {
        public List<HotspotInteractionBase> Interactions;
        public Hotspot Hotspot;
        public Coroutine HotspotRoutine;
    }
    
    public class InteractionSystemHandler : MonoBehaviour
    {
        private Player player;

        private HotspotInteractionBase combinationInteraction;

        private Coroutine hotspotInteractionRoutine;
        
        private Dictionary<InventoryItem, List<HotspotInteractionBase>> inventoryInteractions = new();
        
        public event Action<Hotspot, List<HotspotInteractionBase>> OnHotspotInteraction;
        
        // Keep track of interactions that are queued
        private List<InteractionExecutionData> interactionList = new List<InteractionExecutionData>();
        public bool IsInteracting => interactionList.Count > 0;

        public void Init(Player player)
        {
            this.player = player;
            
            player.OnInteractInput += HotspotInteractInput;
        }

        public void HotspotInteractInput()
        {
            var currentStepOnHotspot = player.CurrentHotspot;
            
            if (currentStepOnHotspot != null  && !currentStepOnHotspot.IsInteracting)
            {
                ExecuteInteraction(currentStepOnHotspot, currentStepOnHotspot.GetInteractions(true));
            }
        }

        public void ExecuteInteraction(Hotspot hotspot, List<HotspotInteractionBase> interactions)
        {
            if (interactions == null) return;
            
            var interactionQueueItem = new InteractionExecutionData
            {
                Hotspot = hotspot,
                Interactions = interactions,
            };
            interactionQueueItem.HotspotRoutine = StartCoroutine(InteractionRoutine(interactionQueueItem));
            
            interactionList.Add(interactionQueueItem);
        }
        
        private IEnumerator InteractionRoutine(InteractionExecutionData interactionExecutionData)
        {
            var interactions = interactionExecutionData.Interactions;
            var hotspot = interactionExecutionData.Hotspot;
            
            yield return hotspot.InteractionRoutine(interactions);
            
            // Remove the interaction from the list
            var interactionQueueItem = interactionList.FirstOrDefault(e => e.Hotspot == hotspot);
            if (interactionQueueItem != null)
            {
                interactionList.Remove(interactionQueueItem);
            }
            
            EvaluateInteractionQueue();
            OnHotspotInteraction?.Invoke(hotspot, interactions);
        }

        private void EvaluateInteractionQueue()
        {
            // Check if there are more interactions in the queue, if not, re enable player movement
            if (interactionList.Count > 0) return;
            //player.SetEnabledHotspotDetector(true);
        }
    }
}