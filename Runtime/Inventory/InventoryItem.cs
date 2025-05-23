namespace DreadZitoEngine.Runtime.Inventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public ItemDataSO Data;
        public int StackSize;
        
        public InventoryItem(ItemDataSO data)
        {
            Data = data;
        }

        public void AddStack(int amount = 1)
        {
            if (StackSize + amount > Data.MaxStackSize)
            {
                StackSize = Data.MaxStackSize;
                return;
            }
            StackSize += amount;
        }
        
        public void RemoveStack(int amount = 1)
        {
            StackSize -= amount;
            if (StackSize < 0)
            {
                StackSize = 0;
            }
        }
    }
}