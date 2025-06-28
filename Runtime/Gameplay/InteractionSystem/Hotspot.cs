using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
            var availableInteractions = interactions.Where(e => e.IsAvailable());
            return isOn && availableInteractions.Any();
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

[CustomEditor(typeof(Hotspot))]
public class HotspotEditor : Editor
{
    private Hotspot hotspot;
    private const string InteractionsFolderPath = "Assets/DreadZitoEngine/Prefabs/InteractionSystem/Interactions/";

    private void OnEnable()
    {
        hotspot = (Hotspot)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        if (GUILayout.Button("Add Interaction"))
        {
            ShowInteractionMenu();
        }
    }

    private void ShowInteractionMenu()
    {
        GenericMenu menu = new GenericMenu();

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { InteractionsFolderPath });
        if (guids.Length == 0)
        {
            menu.AddDisabledItem(new GUIContent("No prefabs found"));
        }
        else
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    string name = prefab.name;
                    menu.AddItem(new GUIContent(name), false, () =>
                    {
                        AddInteractionPrefab(prefab);
                    });
                }
            }
        }

        menu.ShowAsContext();
    }

    private void AddInteractionPrefab(GameObject prefab)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance != null)
        {
            Undo.RegisterCreatedObjectUndo(instance, "Add Interaction Prefab");
            instance.transform.SetParent(hotspot.transform, false);
            instance.name = prefab.name;

            // Posicionamos en el centro del hotspot
            instance.transform.localPosition = Vector3.zero;
            
            hotspot.AddInteraction(instance.GetComponent<HotspotInteractionBase>());

            EditorUtility.SetDirty(hotspot);
        }
    }
}
#endif
}