using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public class CompositeInteraction : HotspotInteractionBase
    {
        [SerializeField] private HotspotInteractionBase[] interactions;
        
        protected override IEnumerator DoInteraction(Hotspot hotspot)
        {
            foreach (var interaction in interactions)
            {
                if (interaction != null) {
                    yield return interaction.ExecuteRoutine(hotspot);
                }
            }
            
            yield return base.DoInteraction(hotspot);
        }
    }
}