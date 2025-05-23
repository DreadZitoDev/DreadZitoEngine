using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public enum AudioBehaviour
    {
        WaitForEnd,
        PlayAndForget,
        PlayAndNotRepeat
    }
    
    [RequireComponent(typeof(AudioSource))]
    public class TriggerAudioInteraction : HotspotInteractionBase
    {
        private AudioSource audioSource;
        public AudioSource AudioSource => audioSource ? audioSource : audioSource = GetComponent<AudioSource>();

        [SerializeField] private AudioBehaviour audioBehaviour = AudioBehaviour.WaitForEnd;
        
        private Coroutine playAudioCoroutine;
        
        internal override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var clip = AudioSource.clip;
            if (!clip)
            {
                Debug.LogError("No audio clip found");
                yield break;
            }
            
            
            if (audioBehaviour == AudioBehaviour.WaitForEnd)
            {
                AudioSource.Play();
                yield return new WaitForSeconds(clip.length);
            }
            else if (audioBehaviour == AudioBehaviour.PlayAndForget)
            {
                AudioSource.Play();
            }
            else if (audioBehaviour == AudioBehaviour.PlayAndNotRepeat && playAudioCoroutine == null)
            {
                playAudioCoroutine = StartCoroutine(PlayAudioCoroutine(clip));
            }
            else
                yield return base.DoInteraction(hotspot);
        }

        private IEnumerator PlayAudioCoroutine(AudioClip clip)
        {
            AudioSource.clip = clip;
            AudioSource.Play();
            yield return new WaitForSeconds(clip.length);
            playAudioCoroutine = null;
        }


        public void SetAudioClip(AudioClip audioClip)
        {
            AudioSource.clip = audioClip;
        }
    }
}