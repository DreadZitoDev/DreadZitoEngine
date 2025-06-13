using DreadZitoEngine.Runtime.Audio.Data;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

namespace DreadZitoEngine.Runtime.FlowCanvasNodes
{
    [Category("DreadZitoEngine")]
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