using System.Collections.Generic;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.QuestManager;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    [Category("Room502")]
    public class DPAddQuestNode : CallableActionNode<List<QuestBase>>
    {
        public override void Invoke(List<QuestBase> quests)
        {
            var questSystem = Game.Instance.QuestsSystem;
            if (quests == null || quests.Count == 0)
                return;
            questSystem.AddQuests(quests.ToArray());
        }
    }
}