using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tools
{
    public class AddQuestTool : MonoBehaviour
    {
        [SerializeField, QuestPopup(true)] private string questName;
        
        public void AddQuest()
        {
            QuestLog.StartQuest(questName);
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AddQuestTool))]
    public class AddQuestToolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var tool = (AddQuestTool) target;
            if (GUILayout.Button("Add Quest"))
            {
                tool.AddQuest();
            }
        }
    }
    #endif
}