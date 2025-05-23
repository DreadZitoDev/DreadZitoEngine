using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Inventory
{
    public class InventorySystem : MonoBehaviour
    {
        private Dictionary<ItemDataSO, InventoryItem> holdingItems = new Dictionary<ItemDataSO, InventoryItem>();
        private List<InventoryItem> inventory = new List<InventoryItem>();
        
        public event Action<InventoryItem> OnItemAdded;
        
        public delegate void RemovedItemDelegate(InventoryItem item, bool stillInInventory);
        public event RemovedItemDelegate OnItemRemoved;
        
        public InventoryItem SelectedItem { get; private set; }
        
        [SerializeField] private CombinationRecipeSO[] recipes;
        
        public CombinationData CombinationData { get; private set; }
        public bool IsInCombinationProcess => CombinationData != null;

        public void AddItem(ItemDataSO itemData, int amount = 1)
        {
            if (holdingItems.TryGetValue(itemData, out var item))
            {
                Debug.Log($"Added {itemData.Name} to inventory");
                item.AddStack(amount);
            }
            else
            {
                Debug.Log($"Created new {itemData.Name} in inventory");
                
                item = new InventoryItem(itemData);
                holdingItems.Add(itemData, item);
                inventory.Add(item);
                item.AddStack(amount);
            }
            
            OnItemAdded?.Invoke(item);
        }
        
        public void RemoveItem(ItemDataSO itemData, int amount = 1)
        {
            if (!holdingItems.TryGetValue(itemData, out var item)) {
                Debug.Log($"Item {itemData.Name} not found in inventory");
                return;
            }
            
            Debug.Log($"Removed {itemData.Name} from inventory");
            item.RemoveStack(amount);
            if (item.StackSize == 0)
            {
                holdingItems.Remove(itemData);
                inventory.Remove(item);
            }
            
            OnItemRemoved?.Invoke(item, inventory.Contains(item));
        }
        
        public InventoryItem[] GetItems() => inventory.ToArray();

        public bool HasItem(ItemDataSO item)
        {
            return holdingItems.ContainsKey(item);
        }
        
        public void SelectItem(InventoryItem item)
        {
            if (SelectedItem == item) return;
            SelectedItem = item;
        }

        public bool HasItems(ItemDataSO[] queryItems)
        {
            // Count inventory items
            Dictionary<ItemDataSO, int> inventoryCounts = GetItems()
                .GroupBy(item => item.Data)
                .ToDictionary(g => g.Key, g => g.Count());

            // Count query items
            Dictionary<ItemDataSO, int> queryCounts = queryItems
                .GroupBy(item => item)
                .ToDictionary(g => g.Key, g => g.Count());

            // Verify quantities
            return queryCounts.All(kvp => 
                inventoryCounts.TryGetValue(kvp.Key, out int count) && 
                count >= kvp.Value
            );
        }
        
        public CombinationData AddToCombination(ItemDataSO item)
        {
            if (CombinationData == null)
            {
                Debug.LogWarning($"No current combination");
                return null;
            }
            CombinationData.EvaluateAddItem(item);
            var data = CombinationData.Copy();
            
            if (CombinationData.IsDone)
            {
                AddItem(CombinationData.Result);
                // remove combined items
                CombinationData.Items.ForEach(item =>
                {
                    var hasAnotherRecipe = recipes.Any(recipe => recipe.Items.Contains(item) &&
                                                                 !HasItem(recipe.Result));
                    if (!hasAnotherRecipe)
                        RemoveItem(item);
                });
                CombinationData = null;
            }
            else if (CombinationData.IsContinue)
            {
                // Continue combination
            }
            else if (CombinationData.IsCancel)
            {
                CombinationData = null;
            }

            return data;
        }

        public CombinationData StartCombination()
        {
            if (SelectedItem == null) {
                Debug.Log($"No item selected");
                return null;
            }
            
            var possibleRecipes = recipes.Where(recipe => recipe.Items.Contains(SelectedItem.Data) && !HasItem(recipe.Result)).ToArray();
            CombinationData = possibleRecipes.Length > 0
                ? new CombinationData()
                {
                    PossibleRecipes = possibleRecipes,
                    Items = new List<ItemDataSO>() {SelectedItem.Data} // Add selected item
                }
                : null;
            return CombinationData;
        }

        public bool HasCombination(InventoryItem selectedItem)
        {
            return recipes.Any(recipe => recipe.Items.Contains(selectedItem.Data) && !HasItem(recipe.Result));
        }

        public AudioTapeItem GetAnyAudioTape()
        {
            return GetItems().FirstOrDefault(item => item.Data is AudioTapeItem)?.Data as AudioTapeItem;
        }
    }

    public enum CombinationState
    {
        None,
        Continue,
        Cancel,
        Done
    }
}