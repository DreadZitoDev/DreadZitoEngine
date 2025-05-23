// Recompile at 21/4/2025 19:00:06
#if USE_TIMELINE
#if UNITY_2017_1_OR_NEWER
// Copyright (c) Pixel Crushers. All rights reserved.

using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PixelCrushers.DialogueSystem
{
    [Serializable]
    public class PlaySequenceClip : PlayableAsset, ITimelineClipAsset
    {
        public PlaySequenceBehaviour template = new PlaySequenceBehaviour();
        public ExposedReference<Transform> listener;

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PlaySequenceBehaviour>.Create(graph, template);
            PlaySequenceBehaviour clone = playable.GetBehaviour();
            clone.listener = listener.Resolve(graph.GetResolver());
            return playable;
        }
    }
}
#endif
#endif
