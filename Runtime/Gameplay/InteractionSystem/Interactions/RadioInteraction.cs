using System.Collections;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class RadioInteraction : HotspotInteractionBase
    {
        [SerializeField] private TriggerAudioInteraction triggerAudioInteraction;
        [SerializeField] private RunConversationInteraction runConversationInteraction;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var playerInventory = GameplayMain.Instance.Player.Inventory;
            var anyAudioTape = playerInventory.GetAnyAudioTape();
            
            if (anyAudioTape == null)
            {
                yield return triggerAudioInteraction.ExecuteRoutine(hotspot);
                yield break;
            }
            
            // Save previous audio clip
            var previousAudioClip = triggerAudioInteraction.AudioSource.clip;
            if (anyAudioTape.RemoveItemAfterUse)
                playerInventory.RemoveItem(anyAudioTape);
            
            yield return ProcessAudioTape(hotspot, anyAudioTape);
            
            triggerAudioInteraction.SetAudioClip(previousAudioClip);
        }

        private IEnumerator ProcessAudioTape(Hotspot hotspot, AudioTapeItem audioTape)
        {
            // Is Conversation, trigger conversation
            if (audioTape.IsConversation)
            {
                runConversationInteraction.SetConversation(audioTape.Conversation);
                yield return runConversationInteraction.ExecuteRoutine(hotspot);
                yield break;
            }
            
            // Is Audio, set audio and then trigger audio
            if (audioTape.IsAudio)
                triggerAudioInteraction.SetAudioClip(audioTape.AudioClip);
        
            yield return triggerAudioInteraction.ExecuteRoutine(hotspot);
        }
    }
}