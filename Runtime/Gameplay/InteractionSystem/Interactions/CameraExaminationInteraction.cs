using System.Collections;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay.Players;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class CameraExaminationInteraction : HotspotInteractionBase
    {
        [Space]
        [SerializeField] private CinemachineCamera lookCamera;
        [SerializeField] private bool cameraFadeStart = true;
        [SerializeField] private bool cameraFadeEnd = true;
        
        [Header("Events")]
        public UnityEvent<CameraExaminationInteraction> OnExaminationStart;
        public UnityEvent<CameraExaminationInteraction> OnInteractionIsActive;
        public UnityEvent<CameraExaminationInteraction> OnExaminationEnd;

        private bool interrupt = false;

        [Header("Settings")]
        private bool allowExitInput = true;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var player = GameplayMain.Instance.Player;
            player.PlayerLockMovement(Player.INTERACTION_MOVE_BLOCKER_ID, true);
            player.SetCanInteract(false);
            
            ResetInterrupt();
            
            if (cameraFadeStart)
                yield return Game.Instance.CameraFade.FadeInCameraRoutine(.2f);
            
            lookCamera.Priority = 100;
            
            OnInteractionStartedEvent?.Invoke();
            OnExaminationStart?.Invoke(this);
            
            if (cameraFadeStart)
                yield return Game.Instance.CameraFade.FadeOutCameraRoutine(.2f);

            while (enabled)
            {
                if (interrupt || (allowExitInput && InputBridge.ExamineInteractionLeave.WasPressedThisFrame())) {
                    break;
                }
                
                OnInteractionIsActive?.Invoke(this);
                yield return null;
            }
            
            if (cameraFadeEnd)
                yield return Game.Instance.CameraFade.FadeInCameraRoutine(.2f);

            lookCamera.Priority = -10;
            OnExaminationEnd?.Invoke(this);
            player.PlayerLockMovement(Player.INTERACTION_MOVE_BLOCKER_ID, false);
            player.SetCanInteract(true);
            
            if (cameraFadeEnd)
                yield return Game.Instance.CameraFade.FadeOutCameraRoutine(.2f);
            
            ResetInterrupt();
            OnInteractionExecutedEvent?.Invoke();
        }

        private void ResetInterrupt()
        {
            // Reset interrupt flag
            if (interrupt)
                interrupt = false;
        }

        public void Interrupt()
        {
            interrupt = true;
        }
        
        public void SetAllowExitInput(bool allow)
        {
            allowExitInput = allow;
        }
    }
}