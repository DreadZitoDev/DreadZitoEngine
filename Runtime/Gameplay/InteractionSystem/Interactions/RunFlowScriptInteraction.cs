using System.Collections;
using FlowCanvas;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class RunFlowScriptInteraction : HotspotInteractionBase
    {
        [SerializeField] private FlowScript flowScript;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var finished = false;
            Game.Instance.RunFlowScript(flowScript, () => finished = true);
            yield return new WaitUntil(() => finished);
        }
    }
}