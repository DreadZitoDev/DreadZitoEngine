using DreadZitoEngine.Runtime.Scenes;
using FlowCanvas.Nodes;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    public class UnLoadScene : CallableActionNode<GameSceneData, bool>
    {
        public override void Invoke(GameSceneData sceneData, bool includeLogic)
        {
            Game.Instance.UnLoadScene(sceneData, includeLogic);
        }
    }
}