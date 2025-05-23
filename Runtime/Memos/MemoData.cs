using UnityEngine;

namespace DreadZitoEngine.Runtime.Memos
{
    [CreateAssetMenu(fileName = "Memo", menuName = "Room502/Memo", order = 0)]
    public class MemoData : ScriptableObject
    {
        public string MemoTitle;
        [TextArea] public string MemoText;
        public Sprite MemoBackground;

        [Tooltip("If isn't true, will wait until player triggers Interaction Input")] public bool AutoRead = true;
    }
}