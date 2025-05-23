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
                var asset = clip.asset as FadeCameraStateClip;
                if (asset)
                {
                    asset.template.clipDuration = (float)clip.duration;
                    asset.template.playClip = true;
                }
            }
            return ScriptPlayable<FadeCameraStateBehaviour>.Create(graph, inputCount);
        }
    }
}