using System;
using System.Collections;
using DreadZitoEngine.Runtime.Audio.Data;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Audio
{
    public class BgMusic : MonoBehaviour
    {
        [SerializeField] private AudioSource _primarySource;
        [SerializeField] private AudioSource _secondarySource;
        [SerializeField] private float _crossfadeDuration = 1.5f;

        private Coroutine _fadeCoroutine;
        
        private void CheckInit()
        {
            // Initialize audio sources if not assigned in Inspector
            if (_primarySource == null || _secondarySource == null)
            {
                _primarySource = gameObject.AddComponent<AudioSource>();
                _secondarySource = gameObject.AddComponent<AudioSource>();
                _primarySource.loop = true;
                _secondarySource.loop = true;
            }
        }
        
        public void PlayTrack(MusicTrackDataSO track, float fadeDuration = -1)
        {
            if (track == null)
            {
                Debug.LogWarning("Tried to play a null MusicTrackDataSO!");
                return;
            }
            
            CheckInit();

            float fadeTime = (fadeDuration < 0) ? _crossfadeDuration : fadeDuration;
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }
            _fadeCoroutine = StartCoroutine(FadeToTrack(track, fadeTime));
        }

        private IEnumerator FadeToTrack(MusicTrackDataSO newTrack, float fadeDuration)
        {
            AudioClip newClip = newTrack.Clip;
            if (newClip == null) yield break;

            AudioSource oldSource = _primarySource;
            AudioSource newSource = _secondarySource;

            // Configure new source properties from the ScriptableObject
            newSource.clip = newClip;
            newSource.volume = 0;
            newSource.pitch = newTrack.Pitch;
            newSource.loop = newTrack.Loop;
            newSource.Play();

            float timer = 0;
            while (timer <= fadeDuration)
            {
                timer += Time.deltaTime;
                float t = timer / fadeDuration;

                newSource.volume = Mathf.Lerp(0, newTrack.Volume, t);
                oldSource.volume = Mathf.Lerp(oldSource.volume, 0, t);

                yield return null;
            }

            oldSource.Stop();
            // Swap roles for next transition
            _primarySource = newSource;
            _secondarySource = oldSource;
        }

        public void StopAllMusic()
        {
            _primarySource.Stop();
            _secondarySource.Stop();
        }

        public void StopMusicSmooth(float duration, Action onComplete)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            _fadeCoroutine = StartCoroutine(StopMusicCoroutine(duration, onComplete));
        }

        private IEnumerator StopMusicCoroutine(float duration, Action onComplete)
        {
            float timer = 0;
            while (timer <= duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;

                _primarySource.volume = Mathf.Lerp(_primarySource.volume, 0, t);
                _secondarySource.volume = Mathf.Lerp(_secondarySource.volume, 0, t);

                yield return null;
            }

            _primarySource.Stop();
            _secondarySource.Stop();

            onComplete?.Invoke();
        }
    }
}