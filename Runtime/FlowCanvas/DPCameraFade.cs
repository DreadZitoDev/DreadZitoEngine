using System.ComponentModel;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.CameraCode;
using FlowCanvas.Nodes;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("Room502")]
    public class DPCameraFade : FlowControlNode
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