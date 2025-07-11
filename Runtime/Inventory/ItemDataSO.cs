using UnityEngine;

namespace DreadZitoEngine.Runtime.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "DownfallProject/ItemData", order = 0)]
    public class ItemDataSO : ScriptableObject
    {
        [field:SerializeField] public string Name {get; private set;}
        [field:SerializeField, TextArea] public string Description {get; private set;}
        [field:SerializeField] public Sprite ItemIcon {get; private set;}
        [field:SerializeField] public int MaxStackSize {get; private set;} = 1;

        [field:SerializeField] public AudioClip PickupSFX {get; private set;}
        
        // public GameObject ItemModel;
    }
}