using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Feedbacks
{
    [RequireComponent(typeof(AudioSource))]
    public class SFX : MonoBehaviour
    {
        [SerializeField] private AudioClip sfxPickup;
        [SerializeField] private AudioClip sfxQuestNoteOpen; // Add another for close if needed
        
        private AudioSource audioSource;

        public void Init()
        {
            audioSource = GetComponent<AudioSource>();

            var gameplayMain = GameplayMain.Instance;
            var player = gameplayMain.Player;

            //player.Inventory.OnItemAdded += PlayItemPickup;
            //player.OnQuestNoteToggle += PlayQuestNote;
            
        }
        
        private void PlayQuestNote(bool visible)
        {
            PlaySfx(sfxQuestNoteOpen);
        }

        private void PlayItemPickup(InventoryItem item)
        {
            var clip = item.Data.PickupSFX ? item.Data.PickupSFX : sfxPickup;
            PlaySfx(clip);
        }

        public void PlaySfx(AudioClip sfx)
        {
            audioSource.PlayOneShot(sfx);
        }
    }
}