using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.PauseOrResumeTimeline
{
    [TrackColor(0.1f, 5f, 12.5f)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(PauseOrResumeTimelineClip))]
    public class PauseOrResumeTimelineTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var asset = clip.asset as PauseOrResumeTimelineClip;
                if (asset)
                {
                    asset.template.playClip = true;
                    asset.template.director = go.GetComponent<PlayableDirector>();
                }
            }
            return ScriptPlayable<PauseOrResumeTimelineBehaviour>.Create(graph, inputCount);
        }
    }
}