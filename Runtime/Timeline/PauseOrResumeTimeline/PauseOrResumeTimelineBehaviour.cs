using System;
using UnityEngine;
using UnityEngine.Playables;

namespace DreadZitoEngine.Runtime.Timeline.PauseOrResumeTimeline
{
    public enum PauseOrResumeTimelineAction
    {
        Pause,
        Resume
    }
    
    [Serializable]
    public class PauseOrResumeTimelineBehaviour : PlayableBehaviour
    {
        // This is used to play the clip only once, apparently there is a general behaviour and a new one is created for each clip
        // So we need to identify which behaviours are from clips and which one is the general one
        [System.NonSerialized] public bool playClip;
        [System.NonSerialized] public PlayableDirector director;
        public PauseOrResumeTimelineAction action = PauseOrResumeTimelineAction.Pause;

        private bool isAlreadyPaused;
        
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (playClip)
            {
                // Avoid Pausing the timeline if it was already paused
                if (isAlreadyPaused && action == PauseOrResumeTimelineAction.Pause) {
                    return;
                }
                Handle();
            }
            
            base.OnBehaviourPlay(playable, info);
        }

        private void Handle()
        {
            Debug.Log($"PauseOrResumeTimelineBehaviour: initial check");
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
#endif
            if (director == null)
            {
                Debug.LogError("PauseOrResumeTimelineBehaviour: director is null");
                return;
            }
            
            if (action == PauseOrResumeTimelineAction.Pause)
            {
                director.Pause();
                isAlreadyPaused = true;
            }
            else
            {
                director.Resume();
            }
        }
    }
}