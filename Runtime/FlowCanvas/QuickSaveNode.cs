using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("DreadZitoEngine")]
    public class QuickSaveNode : CallableActionNode
    {
        public override void Invoke()
        {
            var saveSystem = Game.Instance.SaveSystem;
            saveSystem.Save();
        }
    }
}