using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class RunConversationInteraction : HotspotInteractionBase
    {
        [SerializeField, ConversationPopup(true, false)] private string conversation;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            DialogueManager.StartConversation(conversation);

            yield return new WaitUntil(() => !DialogueManager.IsConversationActive);
            yield return base.DoInteraction(hotspot);
        }

        public void SetConversation(string conversation)
        {
            this.conversation = conversation;
        }
    }
}