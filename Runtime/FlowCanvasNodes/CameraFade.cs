using System.ComponentModel;
using DreadZitoEngine.Runtime.CameraCode;
using FlowCanvas.Nodes;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class CameraFade : FlowControlNode
    {
        protected override void RegisterPorts()
        {
            // Inputs
            var fadeMode = AddValueInput<FadeCameraMode>("FadeMode");
            var duration = AddValueInput<float>("Duration");
            
            // Outputs
            var afterFade = AddFlowOutput("After Fade");
            
            AddFlowInput("In", (f) =>
            {
                var cameraFade = Game.Instance.CameraFade;
                
                if (fadeMode.value == FadeCameraMode.FadeIn)
                {
                    cameraFade.FadeIn(duration.value, () => f.Call(afterFade));
                }
                else
                {
                    cameraFade.FadeOut(duration.value, () => f.Call(afterFade));
                }
            });
        }
    }
}