using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public class Hotspot : MonoBehaviour
    {
        [SerializeField] private string hotspotName;
        [SerializeField] private bool isOn = true;
        [SerializeField] private bool isOneTimeUse;

        public string HotspotName => string.IsNullOrEmpty(hotspotName) ? name : hotspotName;

        private bool isInteracting;
        public bool IsInteracting => isInteracting;

        [SerializeField] private List<HotspotInteractionBase> interactions = new List<HotspotInteractionBase>();

        public event Action<Hotspot> OnSelected;
        public event Action<Hotspot> OnDeselected;

        [Header("Events")] public UnityEvent OnInterected;

        private Collider collider;
        public Collider Collider => collider ? collider : collider = GetComponent<Collider>();

        internal virtual void Start()
        {
            SetInteractions(interactions);
        }

        public List<HotspotInteractionBase> GetInteractions(bool onlyTurnedOn = false)
        {
            return onlyTurnedOn ? interactions.FindAll(e => e.IsActive) : interactions;
        }

        public void SetInteractions(List<HotspotInteractionBase> value)
        {
            interactions = value;
            interactions.ForEach(e => e.SetHotspot(this));
        }

        public void AddInteraction(HotspotInteractionBase combinationInteraction)
        {
            if (!interactions.Contains(combinationInteraction))
                interactions.Add(combinationInteraction);

            if (!Equals(combinationInteraction.Hotspot, this))
                combinationInteraction.SetHotspot(this);
        }

        public void RemoveInteraction(HotspotInteractionBase combinationInteraction)
        {
            if (!interactions.Contains(combinationInteraction)) return;

            interactions.Remove(combinationInteraction);
            combinationInteraction.SetHotspot(null);
        }

        public virtual IEnumerator InteractionRoutine(List<HotspotInteractionBase> interactions)
        {
            StartInteraction();
            foreach (var interaction in interactions)
                yield return interaction.ExecuteRoutine(interaction.Hotspot);
            EndInteraction();
        }

        internal virtual void StartInteraction()
        {
            isInteracting = true;
        }

        internal virtual void EndInteraction()
        {
            isInteracting = false;
            if (isOneTimeUse)
                TurnOff();
            OnInterected?.Invoke();
        }

        public bool IsOn()
        {
            return isOn && interactions.Any(e => e.IsActive);
        }

        public void TurnOn()
        {
            isOn = true;
        }

        public void TurnOff()
        {
            isOn = false;
        }

        public HotspotInteractionBase GetItemInteraction(ItemDataSO itemInteraction)
        {
            return interactions.FirstOrDefault(e => e.RequiredItems.Contains(itemInteraction));
        }

        public void ClearInteractions()
        {
            interactions.Clear();
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Hotspot))]
    public class HotspotEditor : UnityEditor.Editor
    {
        private Hotspot hotspot;

        private void OnEnable()
        {
            hotspot = target as Hotspot;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);
            if (GUILayout.Button("Open Interactions Folder"))
            {
                // Opens the folder where the interactions are stored by path IN PROJECT WINDOW
                OpenPrefabFolder();
            }
        }

        private void OpenPrefabFolder()
        {
            MonoBehaviour targetComponent = target as MonoBehaviour;
            if (targetComponent == null) return;

            // Get the prefab instance root
            GameObject prefabInstance = UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(targetComponent);

            if (prefabInstance != null)
            {
                // Get the path to the prefab asset
                string prefabPath = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabInstance);

                // Load the prefab asset
                GameObject prefabAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

                if (prefabAsset != null)
                {
                    // Focus the Project window and select the prefab asset
                    UnityEditor.EditorUtility.FocusProjectWindow();
                    UnityEditor.Selection.activeObject = prefabAsset;
                    UnityEditor.EditorGUIUtility.PingObject(prefabAsset);
                }
                else
                {
                    Debug.LogWarning($"Could not load prefab asset at path: {prefabPath}");
                }
            }
            else
            {
                Debug.LogWarning("Selected object is not part of a prefab instance");
            }
        }
    }
#endif
}