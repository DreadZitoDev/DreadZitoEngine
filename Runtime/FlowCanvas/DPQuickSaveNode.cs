using _Room502.Scripts;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    public class DPQuickSaveNode : CallableActionNode
    {
        [Category("Room502/Save System/Quick Save")]
        public override void Invoke()
        {
            var saveSystem = Game.Instance.SaveSystem;
            saveSystem.Save();
        }
    }
}