using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public class TriggerBase : MonoBehaviour
    {
        public UnityEvent OnTriggerEnterEvent;
        public UnityEvent OnTriggerExitEvent;
        public UnityEvent OnTriggerStayEvent;
        
        [SerializeField] protected bool playerOnly = true;
        [SerializeField] private bool showYellowGizmo = true;
        
        [SerializeField] private bool oneTimeTrigger = false;
        
        private Collider triggerCollider;
        
        private bool triggered;
        
        protected virtual void Awake()
        {
            triggerCollider = GetComponent<Collider>();
            triggerCollider.isTrigger = true;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (oneTimeTrigger && triggered)
            {
                return;
            }
            
            if (playerOnly)
            {
                if (IsTriggeredByPlayer(other))
                {
                    OnTriggerEnterEvent.Invoke();
                    triggered = true;
                }
            }
            else
            {
                OnTriggerEnterEvent.Invoke();
                triggered = true;
            }
        }
        
        protected virtual void OnTriggerStay(Collider other)
        {
            if (oneTimeTrigger && triggered)
            {
                return;
            }
            
            if (playerOnly)
            {
                if (IsTriggeredByPlayer(other))
                {
                    OnTriggerStayEvent.Invoke();
                }
            }
            else
            {
                OnTriggerStayEvent.Invoke();
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (oneTimeTrigger && triggered)
            {
                return;
            }
            
            if (playerOnly)
            {
                if (IsTriggeredByPlayer(other))
                {
                    OnTriggerExitEvent.Invoke();
                }
            }
            else
            {
                OnTriggerExitEvent.Invoke();
            }
        }
        
        internal bool IsTriggeredByPlayer(Collider other)
        {
            return other.CompareTag("Player");
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (showYellowGizmo && TryGetComponent<BoxCollider>(out var boxCollider))
            {
                Gizmos.color = new Color(1, 1, 0, 0.2f);
                // draw box where the trigger will be, take into account this transform rotation
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(boxCollider.center, boxCollider.size);
                // put label on top of the box
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
                
                var label = $"{GetType().Name}";
                var labelSize = new Vector2(0.5f, 0.5f);
                // position label in center of the box, in world space
                var labelPosition = new Vector3(-labelSize.x / 2, boxCollider.size.y / 2 + labelSize.y, -labelSize.y / 2);
                UnityEditor.Handles.Label(transform.position + labelPosition, label);
            }
        }
        #endif
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(TriggerBase), true)]
    public class TriggerBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Trigger Enter"))
            {
                ((TriggerBase) target).OnTriggerEnterEvent.Invoke();
            }
            if (GUILayout.Button("Trigger Stay"))
            {
                ((TriggerBase) target).OnTriggerStayEvent.Invoke();
            }
            if (GUILayout.Button("Trigger Exit"))
            {
                ((TriggerBase) target).OnTriggerExitEvent.Invoke();
            }
        }
    }
    #endif
}