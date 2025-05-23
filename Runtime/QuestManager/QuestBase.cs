using System;
using System.Collections;
using DreadZitoEngine.Runtime.Inventory;
using DreadZitoEngine.Runtime.SavingLoading;
using DreadZitoEngine.Runtime.Scenes;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.QuestManager
{
    /// <summary>
    /// Base class for all quests,
    /// Quests in this project are handled by Dialogue System quests. Each quest state is handled by a QuestBase implementation.
    /// </summary>
    [System.Serializable]
    public abstract class QuestBase : MonoBehaviour, ISaveable
    {
        [SerializeField] public GameSceneData QuestScene;
        [QuestPopup(true), SerializeField] public string QuestName;

        protected Coroutine questRoutine;
        
        public bool IsRunningQuest => QuestLog.GetQuestState(QuestName) != QuestState.Success;
        
        public event Action<QuestBase> OnQuestComplete; 
        
        public virtual void StartQuest()
        {
            QuestLog.StartQuest(QuestName);
            gameObject.SetActive(true);
            
            if (questRoutine != null)
            {
                StopCoroutine(questRoutine);
            }
            questRoutine = StartCoroutine(QuestRoutine());
        }

        public virtual void OnInventoryAdd(InventoryItem invItem)
        {
            
        }

        public virtual void OnInventoryRemove(InventoryItem item, bool stillInInventory)
        {
            
        }


        protected IEnumerator QuestRoutine()
        {
            while (true)
            {
                QuestUpdate();
                yield return null;
            }
        }

        /// <summary>
        /// Called from a coroutine frame by frame to update the quest state
        /// </summary>
        public virtual void QuestUpdate()
        {
            
        }

        // QuestLog messages
        public virtual void OnQuestStateChange(string title)
        {
            if (title != QuestName) return;
        }

        public virtual void CompleteQuest()
        {
            QuestLog.SetQuestState(QuestName, QuestState.Success);
            OnQuestComplete?.Invoke(this);
        }
        
        public virtual Vector3? GetLastSecurePosition()
        {
            return null;
        }
        
        public abstract object CaptureState();
        public abstract void RestoreState(object state, Action onLoadComplete = null);
    }
}