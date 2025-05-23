using _Room502.Scripts;
using DreadZitoEngine.Runtime.Audio.Data;
using FlowCanvas.Nodes;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvas
{
    public class PlayMusicNode : CallableActionNode<MusicTrackDataSO, float>
    {
        public override void Invoke(MusicTrackDataSO musicTrack, float fadeDuration)
        {
            if (musicTrack == null)
            {
                Debug.LogWarning("Tried to play a null MusicTrackDataSO!");
                return;
            }

            Game.Instance.BgMusic.PlayTrack(musicTrack, fadeDuration);
        }
    }
}