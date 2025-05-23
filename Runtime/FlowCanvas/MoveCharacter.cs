using System.Collections;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("DreadZitoEngine")]
    public class MoveCharacter : LatentActionNode<GameObject, Transform, float>
    {
        public override IEnumerator Invoke(GameObject character, Transform target, float speed)
        {
            if (character == null)
            {
                Debug.LogWarning($"No character to move");
                yield break;
            }

            if (target == null)
            {
                Debug.LogWarning($"No target to move to");
                yield break;
            }

            var cut = false;
            while (!cut)
            {
                var direction = target.position - character.transform.position;
                var distance = direction.magnitude;
                var move = direction.normalized * Mathf.Min(speed * Time.deltaTime, distance);
                
                character.transform.position += move;
                
                if (distance < 0.01f)
                {
                    cut = true;
                }
                
                yield return null;
            }
        }
    }
}