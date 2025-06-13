using DreadZitoEngine.Runtime.Scenes;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class LoadSceneControl : FlowControlNode
    {
        protected override void RegisterPorts()
        {
            // Inputs
            var sceneDataInput = AddValueInput<SceneGroup>("Scene Data");
            var fadeCameraInput = AddValueInput<bool>("Fade Camera");
            
            // Outputs
            var afterLoad = AddFlowOutput("After Load");
            var onCameraFadedIn = AddFlowOutput("On Camera Faded In");
            var preFadeOut = AddFlowOutput("Pre Fade Out");

            AddFlowInput("In", (f) =>
            {
                // Value
                var sceneData = sceneDataInput.value;
                var fadeCamera = fadeCameraInput.value;
                
                Game.Instance.LoadScene(sceneData, () =>
                {
                   f.Call(afterLoad);
                }, fadeCamera, () =>
                {
                    f.Call(onCameraFadedIn);
                }, () =>
                {
                    f.Call(preFadeOut);
                });
            });
        }
    }
}