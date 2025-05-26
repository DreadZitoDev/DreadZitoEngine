using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Credits
{
    [Serializable]
    public class EnvironmentData
    {
        public GameObject RoomHolder;
        public Light DirectionalLight;
        public GameObject VolumeHolder;
    }
    
    [Serializable]
    public class CameraData
    {
        public CinemachineCamera CinemachineCamera;
        public CinemachineSplineDolly SplineDolly;
        public int EnvironmentDataIndex;
    }
    
    public class CreditsCutsceneLogic : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private CameraData[] cameraDatas;
        [SerializeField] private EnvironmentData[] cameraEnvironment;
        [SerializeField] private int defaultEnvIndex = 0;

        private Coroutine _splineRoutine;
        private EnvironmentData currentEnvironmentData;

        private Dictionary<CameraData, EnvironmentData> _cameraEnvironmentDict;
        
        private void Awake()
        {
            var cinemachineEvents = FindObjectOfType<CinemachineBrainEvents>();
            if (cinemachineEvents != null)
            {
                cinemachineEvents.CameraActivatedEvent.AddListener(SwitchedCamera);
            }
            
            _cameraEnvironmentDict = cameraDatas.ToDictionary(k => k, v => cameraEnvironment[v.EnvironmentDataIndex]);
        }
        
        public void StartSplineRoutine(float duration)
        {
            if (_splineRoutine != null)
            {
                StopCoroutine(_splineRoutine);
            }
            _splineRoutine = StartCoroutine(SplineRoutine(duration));
            
        }
        
        private void SwitchedCamera(ICinemachineMixer mixer, ICinemachineCamera cam)
        {
            Debug.Log($"[DB] SwitchedCamera: {cam.Name}");
            var camData = (cam as CinemachineCamera) != null ? cameraDatas.FirstOrDefault(k => k.CinemachineCamera == cam as CinemachineCamera) : null;
            if (camData != null && _cameraEnvironmentDict.TryGetValue(camData, out var env))
            {
                SetEnvironment(env);
            }
            else
            {
                SetEnvironment(cameraEnvironment[defaultEnvIndex]);
            }
        }

        private void SetEnvironment(EnvironmentData data)
        {
            if (currentEnvironmentData != null)
            {
                currentEnvironmentData.RoomHolder.SetActive(false);
                currentEnvironmentData.DirectionalLight.enabled = false;
                currentEnvironmentData.VolumeHolder.SetActive(false);
            }
            
            currentEnvironmentData = data;
            currentEnvironmentData.RoomHolder.SetActive(true);
            currentEnvironmentData.DirectionalLight.enabled = true;
            currentEnvironmentData.VolumeHolder.SetActive(true);
        }
        
        private IEnumerator SplineRoutine(float duration)
        {
            var splineDollies = _cameraEnvironmentDict.Select(kv => kv.Key.SplineDolly).ToArray();
            
            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;

                // advance the dollies along the spline
                foreach (var dolly in splineDollies)
                {
                    dolly.CameraPosition = time / duration;
                }
                
                yield return null;
            }
        }

        public void LoadTitleScreen()
        {
            Game.Instance.LoadTitleScreen();
        }
    }
}