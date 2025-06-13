using DreadZitoEngine.Runtime.Cutscenes;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class PlayCutsceneNode : CallableActionNode<CutsceneData>
    {
        public override void Invoke(CutsceneData cutscene)
        {
            var cutsceneSystem = Game.Instance.CutsceneSystem;
            if (cutscene == null)
                return;
            cutsceneSystem.PlayCutscene(cutscene);
        }
    }
}