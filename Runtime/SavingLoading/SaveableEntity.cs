using System;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;

namespace DreadZitoEngine.Runtime.SavingLoading
{
    [RequireComponent(typeof(SceneObjectID))]
    public class SaveableEntity : MonoBehaviour, ISaveable
    {
        private SceneObjectID sceneObjectID;

        public string ID
        {
            get
            {
                sceneObjectID ??= GetComponent<SceneObjectID>();
                return sceneObjectID.ID;
            }
        }

        public object CaptureState()
        {
            // Get all components that implement ISaveable
            // Limitations: only 1 component of each type ISaveable can be saved
            Dictionary<string, object> state = GetComponents<ISaveable>().
                Where(saveable => saveable is not SaveableEntity).
                ToDictionary(k => k.GetType().Name, v => v.CaptureState());
            return state;
        }

        public void RestoreState(object state, Action onLoadComplete = null)
        {
            var stateDict = state.ParseObject<Dictionary<string, object>>();
            
            foreach (var saveable in GetComponents<ISaveable>())
            {
                var typeName = saveable.GetType().Name;
                if (stateDict.TryGetValue(typeName, out var value))
                {
                    saveable.RestoreState(value);
                }
            }
        }
    }
}