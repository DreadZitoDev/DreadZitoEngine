using DreadZitoEngine.Runtime.CameraCode;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Timeline.FadeCamera
{
    public class FadeCameraStateClip : PlayableAsset, ITimelineClipAsset
    {
        public FadeCameraMode mode;
        public FadeMethod method = FadeMethod.OnGUI;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<FadeCameraStateBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.mode = mode;
            behaviour.method = method;
            return playable;
        }

        public ClipCaps clipCaps => ClipCaps.ClipIn;
    }
}