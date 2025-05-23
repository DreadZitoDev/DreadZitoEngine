using System;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;

namespace _Room502.Scripts
{
    public enum MarkerTiming
    {
        None,
        Awake,
        OnEnable,
        Start
    }
    
    public class Marker : MonoBehaviour
    {
        private GameObject markerGraphic;
        
        [Header("Marker Settings")]
        [SerializeField] private MarkerTiming markerTiming = MarkerTiming.None;
        [SerializeField] private ObjectID_HolderSO objectID;
        [SerializeField] private Transform target;

        private void Awake()
        {
            if (markerTiming == MarkerTiming.Awake)
                BringObject();
        }

        private void OnEnable()
        {
            if (markerTiming == MarkerTiming.OnEnable)
                BringObject();
        }

        private void Start()
        {
            markerGraphic = transform.GetChild(0)?.gameObject;
            markerGraphic?.SetActive(false);
            
            if (markerTiming == MarkerTiming.Start)
                BringObject();
        }

        private void BringObject()
        {
            var target = objectID != null ? objectID.FindInstanceInScene().transform : this.target;
            if (target != null)
            {
                target.position = transform.position;
                target.rotation = transform.rotation;
            }
            else
            {
                Debug.LogWarning($"Marker {name} has no target to bring to the marker position.");
            }
        }
    }
}