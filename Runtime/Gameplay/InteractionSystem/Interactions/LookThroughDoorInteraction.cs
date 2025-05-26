using System;
using System.Collections;
using DreadZitoEngine.Runtime.Gameplay.Players;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class LookThroughDoorInteraction : HotspotInteractionBase
    {
        [SerializeField] private CinemachineCamera doorCamera;
        [SerializeField] private Volume doorVolume;
        
        public event Action OnStartLookThroughDoor;
        public event Action OnEndLookThroughDoor;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var player = GameplayMain.Instance.Player;
            
            player.PlayerLockMovement(Player.INTERACTION_MOVE_BLOCKER_ID, true);
            player.SetCanInteract(false);

            OnStartLookThroughDoor?.Invoke();
            
            yield return Game.Instance.CameraFade.FadeInCameraRoutine();
            doorCamera.Priority = 11;
            doorVolume.weight = 1;
            yield return Game.Instance.CameraFade.FadeOutCameraRoutine();

            while (!Input.GetKeyDown(KeyCode.Escape))
            {
                yield return null;
            }
            
            yield return Game.Instance.CameraFade.FadeInCameraRoutine();
            doorCamera.Priority = -1;
            doorVolume.weight = 0;
            yield return Game.Instance.CameraFade.FadeOutCameraRoutine();
            
            player.PlayerLockMovement(Player.INTERACTION_MOVE_BLOCKER_ID, false);
            player.SetCanInteract(true);
            
            OnEndLookThroughDoor?.Invoke();
        }
    }
}