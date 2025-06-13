using DreadZitoEngine.Runtime.Scenes;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class UnLoadSceneGroup : CallableActionNode<SceneGroup>
    {
        public override void Invoke(SceneGroup sceneData)
        {
            Game.Instance.UnLoadScene(sceneData);
        }
    }
}