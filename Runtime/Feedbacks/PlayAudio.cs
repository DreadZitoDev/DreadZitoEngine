using System.Collections;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Feedbacks
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayAudio : MonoBehaviour
    {
        private AudioSource audioSource;

        private Coroutine playAudioCoroutine;
        
        [SerializeField] private Vector2 pitchVariationRange = new Vector2(0.9f, 1.1f);
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayAudioClipOverride(AudioClip clip)
        {
            if (playAudioCoroutine != null)
            {
                StopCoroutine(playAudioCoroutine);
                playAudioCoroutine = null;
            }
            PlayAudioClip(clip);
        }
        
        public void PlayAudioClip(AudioClip clip)
        {
            if (playAudioCoroutine != null)
                return;
            audioSource.pitch = 1f;
            playAudioCoroutine = StartCoroutine(PlayAudioCoroutine(clip));
        }
        
        public void PlayAudioClipWithPitchVariation(AudioClip clip)
        {
            if (playAudioCoroutine != null)
                return;
            audioSource.pitch = UnityEngine.Random.Range(pitchVariationRange.x, pitchVariationRange.y);
            playAudioCoroutine = StartCoroutine(PlayAudioCoroutine(clip));
        }
        
        public void PlayAudioSrc()
        {
            if (playAudioCoroutine != null)
                return;
            playAudioCoroutine = StartCoroutine(PlayAudioCoroutine(audioSource.clip));
        }
        
        IEnumerator PlayAudioCoroutine(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
            yield return new WaitForSeconds(clip.length);
            playAudioCoroutine = null;
        }
    }
}