using System.Collections;
using DreadZitoEngine.Runtime.UI.System.PageAnim;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DreadZitoEngine.Runtime.UI.System
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Panel : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private PanelAnimation panelAnimation;
        [SerializeField] private Selectable firstSelected;
        public Selectable FirstSelected => firstSelected;

        public PanelAnimation PanelAnimation => panelAnimation;
        public bool IsVisible => CanvasGroup.alpha > 0;
        
        public bool ExitOnNewPanelEnter = true;

        [Header("Events")]
        public UnityEvent<Panel> OnEnter;
        public UnityEvent<Panel> OnExit;
        
        private CanvasGroup canvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                Initialize();
                return canvasGroup;
            }
        }

        public bool IsSkippable { get; protected set; } = true;

        private bool initialized = false;
        
        public virtual void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (initialized) return;
            
            if (!panel)
                panel = gameObject;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            
            panelAnimation ??= GetComponent<PanelAnimation>();
            initialized = true;
        }

        public virtual void Enter()
        {
            panel.SetActive(true);
            OnEnter?.Invoke(this);
            if (panelAnimation)
            {
                IsSkippable = false;
                panelAnimation.Enter(this, () =>
                {
                    IsSkippable = true;
                });
            }
            else
                canvasGroup.alpha = 1;
            
            SetFocus();
        }
        
        public virtual void Exit()
        {
            OnExit?.Invoke(this);
            if (panelAnimation)
            {
                IsSkippable = false;
                panelAnimation.Exit(this, () =>
                {
                    IsSkippable = true;
                });
            }
            else
                canvasGroup.alpha = 0;
            
            SetUnFocus();
        }

        public void SetFocus()
        {
            if (firstSelected == null) return;

            StartCoroutine(SetFocusWait());
        }
        private IEnumerator SetFocusWait()
        {
            yield return null;
            // Select the last selected element if it's a child of the panel
            if (EventSystemIsSelectingAnElementFromPanel(out var element))
            {
                Debug.Log($"Focusing panel {name}, element: {element.name}");
                foreach (var selectable in panel.GetComponentsInChildren<Selectable>()) {
                    selectable.interactable = true;
                }
                element.Select();
            }
            // Select the first selected element
            else if (firstSelected)
            {
                Debug.Log($"Focusing panel {name}, first selected: {firstSelected.name}");
                foreach (var selectable in panel.GetComponentsInChildren<Selectable>()) {
                    selectable.interactable = true;
                }
                firstSelected.Select();
            }
        }
        
        public void SetUnFocus()
        {
            Debug.Log($"Unfocusing panel {name}");
            foreach (var selectable in panel.GetComponentsInChildren<Selectable>()) {
                selectable.interactable = false;
            }
            
            if (!EventSystem.current.currentSelectedGameObject) return;

            Debug.Log($"Removing selection from {EventSystem.current.currentSelectedGameObject.name}");
            if (EventSystemIsSelectingAnElementFromPanel(out var element))
                EventSystem.current.SetSelectedGameObject(null);
        }
        
        public void SetFirstSelected(Selectable selectable)
        {
            firstSelected = selectable;
        }

        private bool EventSystemIsSelectingAnElementFromPanel(out Selectable selectable)
        {
            selectable = null;
            if (!EventSystem.current.currentSelectedGameObject) return false;
            selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();
            return selectable && selectable.transform.IsChildOf(panel.transform);
        }
        
        public void SetInteractable(bool interactable)
        {
            foreach (var selectable in panel.GetComponentsInChildren<Selectable>()) {
                selectable.interactable = interactable;
            }
        }
    }
}