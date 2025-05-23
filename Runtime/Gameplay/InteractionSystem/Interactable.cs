using System.Collections;
using _Room502.Scripts;
using FlowCanvas;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public class Interactable : MonoBehaviour
    {
        [SerializeField] private string interactableName;
        [SerializeField] private FlowScript interactionFlowScript;
        
        public string InteractableName => interactableName;
        
        [SerializeField] private bool oneTimeInteract;

        public bool IsActive { get; private set; } = true;
        
        [Space]
        public UnityEvent<Interactable> OnHover;
        public UnityEvent<Interactable> OnInteract;
        public UnityEvent<Interactable> OnLeave;

        private void Awake()
        {
            SetActive(IsActive);
        }

        public virtual void Hover()
        {
            OnHover.Invoke(this);
        }
        
        public IEnumerator Interact()
        {
            var ended = false;
            
            OnInteract.Invoke(this);
            
            if (oneTimeInteract)
                SetActive(false);
            
            if (interactionFlowScript != null)
            {
                Game.Instance.RunFlowScript(interactionFlowScript, () => ended = true);
                yield return new WaitUntil(() => ended);
            }
        }
        
        public virtual void Leave()
        {
            OnLeave.Invoke(this);
        }
        
        public void SetActive(bool active)
        {
            IsActive = active;
            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = active;
        }
    }
}