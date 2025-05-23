using System;
using UnityEngine;
using UnityEngine.Playables;

namespace DreadZitoEngine.Runtime.Timeline.AnimatorState
{
    [Serializable]
    public class AnimatorStateBehaviour : PlayableBehaviour
    {
        public string PlayAnimation;
        public float Speed = 1;

        bool oneTime;
        
        // This is used to play the clip only once, apparently there is a general behaviour and a new one is created for each clip
        // So we need to identify which behaviours are from clips and which one is the general one
        [System.NonSerialized] public bool playClip;
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!playClip) return;
            
            var animator = playerData as UnityEngine.Animator;
            if (!animator)
                animator = (playerData as GameObject)?.GetComponent<Animator>();
            if (!animator)
                animator =(playerData as GameObject)?.GetComponentInChildren<Animator>();
            if (animator == null) return;
            
            animator.speed = Speed;
            animator.Play(PlayAnimation);
            
            playClip = false;
        }
    }
}