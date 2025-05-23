using Unity.Cinemachine;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tools
{
    [RequireComponent(typeof(CinemachineBrain))]
    public class CinemachineBrainSettings : MonoBehaviour
    {
        [SerializeField] private bool disableCameraIfNoVirtualCameras = false;

        private Camera camera;
        private CinemachineBrain brain;

        private void Start()
        {
            camera = GetComponent<Camera>();
            brain = GetComponent<CinemachineBrain>();
        }

        private void Update()
        {
            if (disableCameraIfNoVirtualCameras)
                camera.enabled = brain.ActiveVirtualCamera != null;
        }
    }
}