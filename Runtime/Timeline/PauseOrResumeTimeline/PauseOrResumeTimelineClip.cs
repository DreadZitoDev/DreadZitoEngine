using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.PauseOrResumeTimeline
{
    public class PauseOrResumeTimelineClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] public PauseOrResumeTimelineBehaviour template = new();
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<PauseOrResumeTimelineBehaviour>.Create(graph, template);
        }

        public ClipCaps clipCaps => ClipCaps.ClipIn;
    }
}