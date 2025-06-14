using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.FadeCamera
{
    [TrackColor(0.5f, 0.5f, 12.5f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(FadeCameraStateClip))]
    public class FadeCameraStateTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var fadeClip = clip.asset as FadeCameraStateClip;
                if (fadeClip != null)
                {
                    var playable = (ScriptPlayable<FadeCameraStateBehaviour>)fadeClip.CreatePlayable(graph, go);
                    var behaviour = playable.GetBehaviour();
                    behaviour.clipDuration = (float)clip.duration;
                    behaviour.playClip = true;
                }
            }

            return ScriptPlayable<FadeCameraStateBehaviour>.Create(graph, inputCount);
        }
    }
}