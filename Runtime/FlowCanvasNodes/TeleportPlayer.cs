using System.ComponentModel;
using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Tags;
using FlowCanvas.Nodes;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class TeleportPlayer : CallableActionNode<ObjectID>
    {
        public override void Invoke(ObjectID target)
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