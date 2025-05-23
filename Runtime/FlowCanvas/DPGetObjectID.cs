using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Tags;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("Room502")]
    public class DPGetObjectID : CallableFunctionNode<GameObject, ObjectID_HolderSO, bool>
    {
        public override GameObject Invoke(ObjectID_HolderSO target, bool isPlayer)
        {
            if (isPlayer)
            {
                return GameplayMain.Instance?.Player?.gameObject;
            }
            
            if (target == null)
            {
                Debug.LogWarning($"No target to get");
                return null;
            }
            
            return Game.GetSceneObject(target.ID);
        }
    }
}