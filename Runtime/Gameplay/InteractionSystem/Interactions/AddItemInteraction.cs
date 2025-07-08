using System;
using System.Collections;
using DreadZitoEngine.Runtime.Inventory;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    [Serializable]
    public class AddItemInteractionData
    {
        public string ItemID;
        public bool ModelEnableState;
    }
    
    public class AddItemInteraction : HotspotInteractionBase
    {
        [SerializeField] private ItemDataSO itemData;
        
        [SerializeField] private GameObject disableModel;
        [SerializeField] private ObjectID disableModelID;
        
        private void Start()
        {
            Name = string.IsNullOrEmpty(Name) ? $"Pick up {itemData.Name}" : Name;
        }

        protected override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var player = GameplayMain.Instance.Player;
            player.Inventory.AddItem(itemData);
            
            // Turn off the interaction
            TurnOff();
            
            GetModel()?.SetActive(false);

            yield return null;
        }

        private GameObject GetModel()
        {
            // Disable the model
            if (disableModel)
            {
                return disableModel;
            }
            
            if (disableModelID) {
                var model = Game.GetSceneObject(disableModelID.ID);
                return model;
            }

            Debug.LogWarning("No model to disable");
            return null;
        }

        public override object CaptureState()
        {
            var data = new AddItemInteractionData();
            
            // If a model is set, save it's enabled state
            data.ModelEnableState = GetModel()?.activeSelf ?? false;

            data.ItemID = $"Data/Items/{itemData.name}";
            return data;
        }
        
        public override void RestoreState(object state, Action onLoadComplete = null)
        {
            var data = state.ParseObject<AddItemInteractionData>();
            itemData = Resources.Load<ItemDataSO>(data.ItemID);
            
            // restore enabled state
            GetModel()?.SetActive(data.ModelEnableState);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(AddItemInteraction))]
    public class AddItemInteractionEditor: Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var interaction = (AddItemInteraction)target;
            SerializedProperty targetObj = serializedObject.FindProperty("disableModel");
            SerializedProperty targetObjID = serializedObject.FindProperty("disableModelID");
            
            DrawPropertiesExcluding(serializedObject, "disableModel", "disableModelID");
            
            var animatorValue = targetObj.objectReferenceValue;
            var animatorIDValue = targetObjID.objectReferenceValue;
            
            if (animatorValue == null && animatorIDValue == null)
            {
                EditorGUILayout.HelpBox("Disable scene model after pick up item, leave as null if any is required", MessageType.Info);
                EditorGUILayout.PropertyField(targetObj);
                EditorGUILayout.PropertyField(targetObjID);
            }
            else if (animatorValue != null)
            {
                EditorGUILayout.PropertyField(targetObj);
                targetObjID.objectReferenceValue = null; // Clear animatorID if animator is set
            }
            else if (animatorIDValue != null)
            {
                EditorGUILayout.PropertyField(targetObjID);
                targetObj.objectReferenceValue = null; // Clear animator if animatorID is set
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}