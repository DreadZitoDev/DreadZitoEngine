using System;
using DreadZitoEngine.Runtime.CameraCode;
using UnityEngine;
using UnityEngine.Playables;

namespace DreadZitoEngine.Runtime.Timeline.FadeCamera
{
    [Serializable]
    public class FadeCameraStateBehaviour : PlayableBehaviour
    {
        public FadeCameraMode mode;
        public FadeMethod method = FadeMethod.OnGUI;
        [System.NonSerialized] public float clipDuration;
        // This is used to play the clip only once, apparently there is a general behaviour and a new one is created for each clip
        // So we need to identify which behaviours are from clips and which one is the general one
        [System.NonSerialized] public bool playClip;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (playClip)
                FadeCamera();
            
            base.OnBehaviourPlay(playable, info);
        }

        private void FadeCamera()
        {
            Debug.Log($"FadeCameraStateBehaviour: initial check");
#if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                return;
            }
#endif
            var cameraFade = Game.Instance.CameraFade;

            Debug.Log($"FadeCameraStateBehaviour: FadeCamera {mode} with duration {clipDuration}");
            if (mode == FadeCameraMode.FadeIn)
            {
                cameraFade.FadeIn(clipDuration, method: method);
            }
            else
            {
                cameraFade.FadeOut(clipDuration, method: method);
            }
        }
    }
}