using UnityEngine;

namespace DreadZitoEngine.Runtime.Audio.Data
{
    [CreateAssetMenu(fileName = "MusicTrack", menuName = "Room502/Music Track", order = 0)]
    public class MusicTrackDataSO : ScriptableObject
    {
        public AudioClip Clip;
        [Range(0, 1)] public float Volume = 1f;
        [Range(0.1f, 3)] public float Pitch = 1f;
    	public bool Loop = true;
    }
}