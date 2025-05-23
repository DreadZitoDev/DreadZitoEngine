using System;
using System.Collections;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Inventory;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class RemoveItemInteraction : HotspotInteractionBase
    {
        [SerializeField] private ItemDataSO itemData;
        
        [SerializeField, Tooltip("Disable scene model after pick up item, leave as null if any is required")]
        private ObjectID disableModelID;

        [SerializeField] private GameObject disableModel;
        
        private void Start()
        {
            Name = string.IsNullOrEmpty(Name) ? $"Pick up {itemData.Name}" : Name;
        }

        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var player = GameplayMain.Instance.Player;
            player.Inventory.RemoveItem(itemData);
            
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
}