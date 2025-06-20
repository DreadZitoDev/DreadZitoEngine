using System.Collections.Generic;
using DreadZitoEngine.Runtime.QuestManager;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
    public class AddQuestNode : CallableActionNode<List<QuestBase>>
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