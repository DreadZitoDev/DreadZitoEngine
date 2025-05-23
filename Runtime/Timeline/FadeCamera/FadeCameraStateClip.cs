using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.FadeCamera
{
    public class FadeCameraStateClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] public FadeCameraStateBehaviour template = new();
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<FadeCameraStateBehaviour>.Create(graph, template);
        }

        public ClipCaps clipCaps => ClipCaps.ClipIn;
    }
}