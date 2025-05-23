using DreadZitoTools.ScriptableLovers;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Inventory
{
    [CreateAssetMenu(fileName = "AudioTape", menuName = "DownfallProject/AudioTape", order = 0)]
    [ScriptableObjectPath("Assets/_Room5002/Data/Items/")]
    public class AudioTapeItem : ItemDataSO
    {
        [ConversationPopup(true, false)] public string Conversation; 
        public AudioClip AudioClip;
        
        public bool RemoveItemAfterUse = true;
        
        public bool IsConversation => !string.IsNullOrEmpty(Conversation);
        public bool IsAudio => AudioClip != null;
    }
}