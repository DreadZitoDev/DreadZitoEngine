using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.AnimatorState
{
    [TrackColor(1f, 0.5f, 12.5f)]
    [TrackBindingType(typeof(Animator))]
    [TrackClipType(typeof(AnimatorStateClip))]
    public class AnimatorStateTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var asset = clip.asset as AnimatorStateClip;
                if (asset)
                {
                    asset.template.playClip = true;
                }
            }
            return ScriptPlayable<AnimatorStateBehaviour>.Create(graph, inputCount);
        }
    }
}