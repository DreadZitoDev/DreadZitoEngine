using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Tags;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class GetObjectID : CallableFunctionNode<GameObject, ObjectID, bool>
    {
        public override GameObject Invoke(ObjectID target, bool isPlayer)
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