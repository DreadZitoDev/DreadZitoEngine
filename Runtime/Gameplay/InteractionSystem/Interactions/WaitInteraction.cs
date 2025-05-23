using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class WaitInteraction : HotspotInteractionBase
    {
        [Space]
        [SerializeField] private float waitTime = 1f;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            yield return new WaitForSeconds(waitTime);
            yield return base.DoInteraction(hotspot);
        }
    }
}