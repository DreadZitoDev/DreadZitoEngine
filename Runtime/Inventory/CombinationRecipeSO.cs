using UnityEngine;

namespace DreadZitoEngine.Runtime.Inventory
{
    [CreateAssetMenu(fileName = "CombinationRecipe", menuName = "DownfallProject/CombinationRecipe", order = 0)]
    public class CombinationRecipeSO : ScriptableObject
    {
        [SerializeField] private ItemDataSO result;
        [SerializeField] private ItemDataSO[] items;
        public ItemDataSO Result => result;
        public ItemDataSO[] Items => items;
    }
}