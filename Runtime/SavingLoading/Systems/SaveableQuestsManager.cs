using System;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.QuestManager;
using UnityEngine;

namespace DreadZitoEngine.Runtime.SavingLoading.Systems
{
    [RequireComponent(typeof(QuestsSystem))]
    public class SaveableQuestsManager : MonoBehaviour, ISaveable
    {
        private QuestsSystem _questsSystem;
        
        public object CaptureState()
        {
            _questsSystem ??= GetComponent<QuestsSystem>();
            Dictionary<string,object> questsState = _questsSystem.ActiveQuests.ToDictionary(k => k.QuestName, v => v.CaptureState());
            return questsState;
        }

        public void RestoreState(object state, Action onLoadComplete)
        {
            _questsSystem ??= GetComponent<QuestsSystem>();
            
            var questsState = state.ParseObject<Dictionary<string, object>>();
            var loadedQuests = 0;
            
            if (questsState.Count == 0)
            {
                onLoadComplete?.Invoke();
                return;
            }
            
            foreach (var kv in questsState)
            {
                var questName = kv.Key;
                var questState = kv.Value;
                
                _questsSystem.StartQuest(questName, quest =>
                {
                    quest.RestoreState(questState);
                    loadedQuests++;
                    if (loadedQuests >= questsState.Count)
                    {
                        onLoadComplete?.Invoke();
                    }
                });
            }
        }
    }
}