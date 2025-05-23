using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tools
{
    public class InventoryTools : MonoBehaviour
    {
        [SerializeField] private ItemDataSO itemData;
        [SerializeField] private ItemDataSO[] items;
        [SerializeField] private bool useList;

        [SerializeField] private bool setItemsOnStart;
        
        private GameplayMain gm => GameplayMain.Instance;

        private void Start()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (setItemsOnStart)
                AddItemToInventory();
#endif
        }

        public void AddItemToInventory()
        {
            if (!useList)
                gm.Player.AddItem(itemData);
            else
                foreach (var item in items)
                    gm.Player.AddItem(item);
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(InventoryTools))]
    public class InventoryToolsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var inventoryTools = (InventoryTools) target;
            if (GUILayout.Button("Add item to inventory"))
            {
                inventoryTools.AddItemToInventory();
            }
        }
    }
    #endif
}