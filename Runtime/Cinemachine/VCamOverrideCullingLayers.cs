using Unity.Cinemachine;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Cinemachine
{
    [RequireComponent(typeof(CinemachineCameraEvents))]
    public class VCamOverrideCullingLayers : MonoBehaviour
    {
        private CinemachineCameraEvents _cameraEvents;

        public LayerMask cullingMaskWhileLive;
        private LayerMask _savedLayerMask;
        
        // Flag to prevent SwitchCamera without previously modifying the culling mask sets empty culling mask
        private bool cullingMaskModified = false;
        
        private void Awake()
        {
            _cameraEvents = GetComponent<CinemachineCameraEvents>();
        }

        private void OnEnable()
        {
            _cameraEvents.CameraActivatedEvent.AddListener(OnTransitionFromCamera);
            _cameraEvents.CameraDeactivatedEvent.AddListener(RestoreCullingMask);
        }

        private void OnDisable()
        {
            _cameraEvents.CameraActivatedEvent.RemoveListener(OnTransitionFromCamera);
            _cameraEvents.CameraDeactivatedEvent.RemoveListener(RestoreCullingMask);
        }

        private void OnTransitionFromCamera(ICinemachineCamera fromCamera, ICinemachineCamera toCamera)
        {
            _savedLayerMask = UnityEngine.Camera.main.cullingMask;
            UnityEngine.Camera.main.cullingMask = cullingMaskWhileLive;
            cullingMaskModified = true;
        }
        
        
        private void RestoreCullingMask(ICinemachineMixer arg0, ICinemachineCamera arg1)
        {
            RestoreCullingMask();
        }
        
        private void RestoreCullingMask()
        {
            if (!cullingMaskModified) return;
            
            UnityEngine.Camera.main.cullingMask = _savedLayerMask;
            cullingMaskModified = false;
        }
    }
}