using System;
using System.Linq;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tools
{
    public class ArtificialLight : MonoBehaviour
    {
        [SerializeField] private GameObject[] lights;
        [SerializeField] private Material lightMaterial;
        [SerializeField] private MeshRenderer meshRenderer;

        [Header("Settings")]
        [SerializeField] private bool startsOn;
        [SerializeField] private float turnedOnEmissionIntensity = 1;
        
        private int runtimeMatIndex = -1;
        private bool isOn;
        
        private void Start()
        {
            if (!meshRenderer)
                meshRenderer = GetComponent<MeshRenderer>();
            
            var mat = meshRenderer.sharedMaterials.FirstOrDefault(e => e == lightMaterial);
            if (!mat) return;
            
            runtimeMatIndex = Array.IndexOf(meshRenderer.sharedMaterials, mat);
            
            if (startsOn)
                TurnOn();
            else 
                TurnOff();
        }

        public void Switch()
        {
            Switch(!isOn);
        }
        
        public void TurnOn()
        {
            Switch(true);
        }
        
        public void TurnOff()
        {
            Switch(false);
        }

        public bool IsOn => isOn;
        
        private void Switch(bool on)
        {
            isOn = on;
            foreach (var light in lights)
            {
                light.SetActive(on);
            }

            if (runtimeMatIndex <= -1) return;
            var mat = meshRenderer.materials[runtimeMatIndex];
            mat.SetFloat("_EmissiveIntensity", on ? turnedOnEmissionIntensity : 0);
            mat.SetColor("_EmissiveColor", on ? Color.white : Color.black);
        }
    }
    
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ArtificialLight))]
    public class ArtificialLightEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var light = (ArtificialLight) target;
            if (GUILayout.Button("Turn On"))
                light.TurnOn();
            if (GUILayout.Button("Turn Off"))
                light.TurnOff();
            if (GUILayout.Button("Switch"))
                light.Switch();
        }
    }
#endif
}