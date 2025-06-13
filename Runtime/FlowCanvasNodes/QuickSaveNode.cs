using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    public class QuickSaveNode : CallableActionNode
    {
        [Category("Room502/Save System/Quick Save")]
        public override void Invoke()
        {
            var saveSystem = Game.Instance.SaveSystem;
            saveSystem.Save();
        }
    }
}