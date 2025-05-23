using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.AnimatorState
{
    public class AnimatorStateClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField] public AnimatorStateBehaviour template = new AnimatorStateBehaviour();
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<AnimatorStateBehaviour>.Create(graph, template);
        }

        public ClipCaps clipCaps => ClipCaps.ClipIn;
    }
}