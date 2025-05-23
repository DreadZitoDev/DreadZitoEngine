using System.Collections;
using DreadZitoEngine.Runtime.Gameplay.Players;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class PlayerMovementInteraction : HotspotInteractionBase
    {
        [Header("Settings")]
        [SerializeField] private bool lockMovement = false;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var player = GameplayMain.Instance.Player;
            player.PlayerLockMovement(Player.INTERACTION_MOVE_BLOCKER_ID, lockMovement);
            
            yield return base.DoInteraction(hotspot);
        }
    }
}