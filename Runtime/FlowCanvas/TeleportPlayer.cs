using System.ComponentModel;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Tags;
using FlowCanvas.Nodes;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("DreadZitoEngine")]
    public class TeleportPlayer : CallableActionNode<ObjectID_HolderSO>
    {
        public override void Invoke(ObjectID_HolderSO target)
        {
            if (target == null)
            {
                Debug.LogWarning($"No target to teleport to");
                return;
            }
            
            var runtimeTarget = Game.GetSceneObject(target.ID);
            var player = GameplayMain.Instance.Player;
            
            player.TeleportTo(runtimeTarget.transform);
        }
    }
}