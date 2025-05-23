using System;
using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.Players
{
    [Serializable]
    public class FootstepData
    {
        public string Tag;
        public AudioClip[] Footsteps;
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class PlayerFootsteps : MonoBehaviour
    {
        [SerializeField] private FootstepData[] footstepData;
        [SerializeField] private float minSecondsMovement = 0.5f;
        [SerializeField, Tooltip("Delay between clips")] private float footstepDelay = 0.5f;
        
        private AudioSource audioSource;

        private FootstepData currentFootstepData;

        private Coroutine footstepRoutine;


        private void OnEnable()
        {
            footstepRoutine = StartCoroutine(PlayFootstep());
        }

        private void OnCollisionEnter(Collision other)
        {
            var anyFootstep = Array.Find(footstepData, data => other.gameObject.CompareTag(data.Tag));
            if (anyFootstep == null) return;
            
            currentFootstepData = anyFootstep;
        }

        private IEnumerator PlayFootstep()
        {
            audioSource ??= GetComponent<AudioSource>();

            var cut = enabled;
            while (cut)
            {
                if (currentFootstepData == null)
                {
                    yield return null;
                    continue;
                }

                var footstepsList = currentFootstepData.Footsteps;
                for (int i = 0; i < footstepsList.Length; i++)
                {
                    // Wait until the player is moving enough time
                    var t = 0f;
                    while (t < minSecondsMovement)
                    {
                        if (GameplayMain.Instance.Player.IsMoving)
                            t += Time.deltaTime;
                        else
                            t = 0f;
                        yield return null;
                    }
                    
                    yield return new WaitForSeconds(footstepDelay);
                    
                    // Play the clip
                    var clip = footstepsList[i];
                    audioSource.clip = clip;
                    audioSource.Play();
                    
                    yield return new WaitForSeconds(clip.length);
                    // Check if the component is still enabled
                    cut = enabled;
                    if (!cut) 
                        break;
                }
                
                yield return null;
            }
        }
    }
}