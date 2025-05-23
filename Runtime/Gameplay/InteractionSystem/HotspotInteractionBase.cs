using System;
using System.Collections;
using DreadZitoEngine.Runtime.Inventory;
using DreadZitoEngine.Runtime.SavingLoading;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public abstract class HotspotInteractionBase : MonoBehaviour, ISaveable
    {
        [HideInInspector] public Hotspot Hotspot;
        [field:SerializeField] public string Name { get; protected set; }

        [SerializeField] private bool isTurnOn = true;
        public bool IsActive => isTurnOn /*&& (requiredItems.Length <= 0 || GameplayMain.Instance.Player.Inventory.HasItems(requiredItems))*/;

        [SerializeField] private bool turnOffAfterInteraction;
        
        [Header("Require Item")]
        [SerializeField] private ItemDataSO[] requiredItems;
        public ItemDataSO[] RequiredItems => requiredItems;
        [SerializeField, Tooltip("Used when required item is set. Remove used items from the inventory after trigger this interaction")]
        private bool removeItemAfterInteraction = true;

        [Header("Event")]
        [SerializeField] private UnityEvent OnNotHaveRequiredItems;

        public UnityEvent OnInteractionStartedEvent;
        public UnityEvent OnInteractionExecutedEvent;
        
        public event Action OnInteractionStarted;
        public event Action OnInteractionExecuted;
        
        /// <summary>
        /// This field is used to store the item that holds this interaction if it's an item interaction.
        /// </summary>
        public ItemDataSO ItemInteraction { get; private set; }
        /// <summary>
        /// Set this interaction to be an item interaction and hold the item reference
        /// </summary>
        /// <param name="item">Item that this interaction belongs to</param>
        public void SetItemInteraction(ItemDataSO item)
        {
            ItemInteraction = item;
        }
        
        public virtual void SetHotspot(Hotspot hotspot)
        {
            this.Hotspot = hotspot;
        }
        
        public IEnumerator ExecuteRoutine(Hotspot hotspot)
        {
            // THIS IS MEANT TO BE OVERRIDEN AND EXECUTED AFTER THE CUSTOM IMPLEMENTATION OF THE INTERACTION
            if (requiredItems.Length > 0 && !GameplayMain.Instance.Player.Inventory.HasItems(requiredItems))
            {
                Debug.Log("Missing required items");
                OnNotHaveRequiredItems?.Invoke();
                yield break;
            }
            
            RemoveRequiredItem();

            OnInteractionStarted?.Invoke();
            OnInteractionStartedEvent?.Invoke();
            
            yield return DoInteraction(hotspot);
            
            if (turnOffAfterInteraction)
                SetActive(false);
            
            OnInteractionExecuted?.Invoke();
            OnInteractionExecutedEvent?.Invoke();
        }

        internal virtual IEnumerator DoInteraction(Hotspot hotspot)
        {
            // THIS IS MEANT TO BE OVERRIDEN
            yield break;
        }
        
        internal void RemoveRequiredItem()
        {
            if (!removeItemAfterInteraction || requiredItems.Length <= 0) return;
            
            foreach (var item in requiredItems)
            {
                GameplayMain.Instance.Player.Inventory.RemoveItem(item);
            }
            // remove item requirement after used
            requiredItems = Array.Empty<ItemDataSO>();
        }
        
        public void SetActive(bool value)
        {
            isTurnOn = value;
        }

        public void TurnOn()
        {
            isTurnOn = true;
        }
        
        public void TurnOff()
        {
            isTurnOn = false;
        }

        public virtual object CaptureState()
        {
            // TODO: not a very good practice
            // MAY BE OVERRIDEN
            return null;
        }

        public virtual void RestoreState(object state, Action onLoadComplete = null)
        {
            // MAY BE OVERRIDEN
        }
    }
}