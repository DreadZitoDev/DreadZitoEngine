using System;
using System.Linq;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay.Players;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;

namespace DreadZitoEngine.Runtime.SavingLoading.Entities
{
    [Serializable]
    public class PlayerSaveData
    {
        public PlayerSaveData() { }
        
        public PlayerSaveData(Player player)
        {
            var pos = player.transform.position;
            var rot = player.transform.rotation;
            
            Position = new[] {pos.x, pos.y, pos.z};
            Rotation = new[] {rot.x, rot.y, rot.z, rot.w};
            HoldingItems = player.Inventory.GetItems().Select(invItem => new InventoryItemData()
            {
                ItemPath = $"Data/Items/{invItem.Data.name}",
                StackSize = invItem.StackSize
            }).ToArray();
        }
        
        public float[] Position;
        public float[] Rotation;
        
        public InventoryItemData[] HoldingItems;

        public Vector3 GetPosition() => new Vector3(Position[0], Position[1], Position[2]);
        public Quaternion GetRotation() => new Quaternion(Rotation[0], Rotation[1], Rotation[2], Rotation[3]);
    }
    
    [Serializable]
    public class InventoryItemData
    {
        public string ItemPath;
        public int StackSize;
    }
    
    [RequireComponent(typeof(Player))]
    public class SaveablePlayer : MonoBehaviour, ISaveable
    {
        private Player player;
        private InventorySystem inventorySystem;
        
        public object CaptureState()
        {
            player ??= GetComponent<Player>();
            inventorySystem ??= GetComponent<InventorySystem>();
            
            return new PlayerSaveData(player);
        }

        public void RestoreState(object state, Action onLoadComplete = null)
        {
            var playerSaveData = state.ParseObject<PlayerSaveData>();
            player ??= GetComponent<Player>();
            inventorySystem ??= GetComponent<InventorySystem>();

            // Restore player position and rotation
            player.transform.position = playerSaveData.GetPosition();
            player.transform.rotation = playerSaveData.GetRotation();
            
            // Restore inventory items
            var inventoryItems = playerSaveData.HoldingItems;
            foreach (var itemData in inventoryItems)
            {
                inventorySystem.AddItem(Resources.Load<ItemDataSO>(itemData.ItemPath), itemData.StackSize);
            }
        }
    }
}