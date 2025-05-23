using System;
using System.Linq;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Dialogue
{
    public class DialogueSystemHandler : MonoBehaviour
    {
        [SerializeField] private DialogueSystemController dialogueSystemController;
        [SerializeField] private DialogueSystemEvents dialogueSystemEvents;
        
        public event Action<string[]> OnReceivedTrackerUpdate; 
     
        public bool IsRunningConversation => DialogueManager.IsConversationActive;
        
        private void Awake()
        {
            dialogueSystemController ??= GetComponentInChildren<DialogueSystemController>();
            dialogueSystemController.receivedUpdateTracker += ReceivedUpdateTracker;
        }

        private void ReceivedUpdateTracker()
        {
            var activeQuests = QuestLog.GetAllQuests().Select(quest =>
            {
                var displayName = QuestLog.GetQuestTitle(quest);
                var questEntry = GetActiveEntry(quest);
                
                var hasDisplayName = !string.IsNullOrEmpty(displayName);
                var hasQuestEntry = !string.IsNullOrEmpty(questEntry);
                
                return hasQuestEntry ? questEntry : 
                        (hasDisplayName ? displayName : quest);
            }).ToArray();
            Debug.Log($"Received tracker update: {string.Join(", ", activeQuests)}");
            OnReceivedTrackerUpdate?.Invoke(activeQuests);
        }

        private string GetActiveEntry(string quest)
        {
            for (int i = 0; i <= QuestLog.GetQuestEntryCount(quest); i++)
            {
                var state = QuestLog.GetQuestEntryState(quest, i);
                if (state == QuestState.Active)
                {
                    return QuestLog.GetQuestEntry(quest, i);
                }
            }

            return string.Empty;
        }

        public void PlayBarkSequential(string barkConversation, float timeBetweenBarks)
        {
            var conversation = DialogueManager.MasterDatabase.GetConversation(barkConversation);
            if (conversation == null)
            {
                Debug.LogError($"Conversation {barkConversation} not found in database.");
                return;
            }
            
            
        }
        
        private Transform registeredPlayerSubject;
    }
}