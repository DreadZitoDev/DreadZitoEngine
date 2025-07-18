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

        public virtual void AddItem(ItemDataSO itemData, int amount = 1)
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
        
        public virtual void RemoveItem(ItemDataSO itemData, int amount = 1)
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

        public InventoryItem GetItem(ItemDataSO item) => holdingItems.GetValueOrDefault(item);

        public bool HasItem(ItemDataSO item)
        {
            return holdingItems.ContainsKey(item);
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
        
    }
}