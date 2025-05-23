using System;
using System.Collections.Generic;
using System.Linq;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using UnityEngine;

namespace DreadZitoEngine.Runtime.QuestManager
{
    public class QuestsSystem : MonoBehaviour
    {
        // List of all quests in the game, each quest is a prefab with a QuestBase implementation
        [SerializeField] private List<QuestBase> quests = new List<QuestBase>();
        public event Action<QuestBase> OnQuestStarted;
        public List<QuestBase> Quests => quests;

        public List<QuestBase> ActiveQuests { get; private set; } = new List<QuestBase>();

        public event Action<QuestBase> OnQuestCompleted; 
        
        public void StartQuest(QuestBase quest, Action<QuestBase> onLoadQuest = null)
        {
            if (ActiveQuests.Contains(quest)) {
                Debug.LogError($"Quest {quest.QuestName} is already active");
                return;
            }
            
            if (!quests.Contains(quest))
                quests.Add(quest);
            
            StartQuest(quest.QuestName, onLoadQuest);
        }
        
        public void StartQuest(string questName, Action<QuestBase> onLoadQuest = null)
        {
            var questPrefab = quests.Find(q => q.QuestName == questName);
            if (questPrefab == null)
            {
                Debug.LogError($"Quest with name {questName} not found");
                return;
            }

            var questScene = questPrefab.QuestScene;
            var quest = FindObjectsOfType<QuestBase>().FirstOrDefault(q => q.QuestName == questName);
            if (quest != null)
            {
                SetupQuest(quest, onLoadQuest);
                return;
            }
            
            Game.Instance.LoadScene(questScene, () =>
            {
                quest = FindObjectsOfType<QuestBase>().FirstOrDefault(q => q.QuestName == questName);
                SetupQuest(quest, onLoadQuest);
            });
        }

        private void SetupQuest(QuestBase quest, Action<QuestBase> onLoadQuest)
        {
            var inventory = GameplayMain.Instance.Player.Inventory;
            
            ActiveQuests.Add(quest);
            inventory.OnItemAdded += (item) => quest.OnInventoryAdd(item);
            inventory.OnItemRemoved += (item, stillInInventory) => quest.OnInventoryRemove(item, stillInInventory);
            //GameplayMain.Instance.OnHotspotInteract += (hotspot) => quest.OnHotspotInteract(hotspot);
            
            onLoadQuest?.Invoke(quest);
            quest.OnQuestComplete += QuestCompleted;
            quest.StartQuest();
            OnQuestStarted?.Invoke(quest);
        }

        private void QuestCompleted(QuestBase quest)
        {
            ActiveQuests.Remove(quest);
            Destroy(quest.gameObject);
            //TODO: Consider unload quest scene and split quests into individual scenes
            
            OnQuestCompleted?.Invoke(quest);
        }

        public void AddQuests(QuestBase[] initialQuests)
        {
            foreach (var quest in initialQuests)
            {
                StartQuest(quest.QuestName);
            }
        }

        public void Reset()
        {
            foreach (var quest in ActiveQuests)
            {
                Destroy(quest.gameObject);
            }
            ActiveQuests.Clear();
        }
    }
}