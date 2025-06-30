using System.Collections;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class UnityEventInteraction : HotspotInteractionBase
    {
        public UnityEvent OnInteractionExecuted;
        
        protected override IEnumerator DoInteraction(Hotspot hotspot)
        {
            OnInteractionExecuted.Invoke();
            yield break;
        }
    }
}