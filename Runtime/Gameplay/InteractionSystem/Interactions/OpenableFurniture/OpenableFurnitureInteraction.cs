using System.Collections;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions.OpenableFurniture
{
    public abstract class OpenableFurnitureInteraction : HotspotInteractionBase
    {
        [SerializeField, Tooltip("If no transform set, look this")] protected ObjectID furnitureID;
        [SerializeField] protected Transform furnitureTransform;
        [SerializeField] private bool startOpen = false;
        [SerializeField] protected float openDuration = 1f;
        [SerializeField] protected bool isLocked;
        
        [Space]
        public UnityEvent OnLocked;

        internal bool runtimeIsOpen = false;

        public virtual void Awake()
        {
            runtimeIsOpen = startOpen;
        }

        public void SetLocked(bool locked)
        {
            isLocked = locked;
        }
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            if (isLocked)
            {
                OnLocked?.Invoke();
                yield break;
            }
            
            if (runtimeIsOpen)
                yield return CloseRoutine(hotspot);
            else
                yield return OpenRoutine(hotspot);
        }

        public abstract IEnumerator CloseRoutine(Hotspot hotspot);
        public abstract IEnumerator OpenRoutine(Hotspot hotspot);
    }
}