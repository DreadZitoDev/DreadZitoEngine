using System.Collections;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Tools
{
    public class PositionsLerper : MonoBehaviour
    {
        public UnityEvent<float> OnProgress;
        
        [SerializeField] public Transform startPosition;
        [SerializeField] public Transform endPosition;
        
        private Coroutine lerpRoutine;
        public float progressDebug;
        
        public bool localSpace = false;

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Create start and end positions if they are null
            if (startPosition == null || endPosition == null)
            {
                var start = new GameObject("Start Position");
                var end = new GameObject("End Position");
                
                start.transform.SetParent(transform);
                end.transform.SetParent(transform);
                
                start.transform.localPosition = Vector3.zero;
                end.transform.localPosition = Vector3.zero;
                
                startPosition = start.transform;
                endPosition = end.transform;
            }
        }
#endif

        public void StartLerp()
        {
            lerpRoutine = StartCoroutine(LerpRoutine());
        }

        private IEnumerator LerpRoutine()
        {
            var player = GameplayMain.Instance.Player;
            var inverse = 0f;
            
            while (enabled)
            {
                var startPos = localSpace ? startPosition.localPosition : startPosition.position;
                var endPos = localSpace ? endPosition.localPosition : endPosition.position;
                
                inverse = Utils.Vector3InverseLerp(startPos, endPos, player.GetPosition());
                progressDebug = inverse;
                
                OnProgress?.Invoke(inverse);
                yield return null;
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartLerp();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StopCoroutine(lerpRoutine);
            }
        }
    }
}