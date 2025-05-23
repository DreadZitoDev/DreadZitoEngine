using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Dialogue
{
    public class VoiceOverHandler : MonoBehaviour
    {
        private void Start()
        {
            DialogueDatabase.getCustomEntrytag += GetCustomEntryTag;
        }

        private string GetCustomEntryTag(Conversation conversation, DialogueEntry entry)
        {
            var actor = DialogueManager.masterDatabase.GetActor(entry.ActorID);
            var entryTag = $"VoiceOver/Eng/{conversation.Title}/{actor.Name}_{conversation.id}_{entry.id}";
            Debug.Log($"entryTag: {entryTag}");
            return entryTag;
        }

        private void OnDestroy()
        {
            DialogueDatabase.getCustomEntrytag -= GetCustomEntryTag;
        }
    }
}