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
        public FadeMethod method;
        public float clipDuration;
        public bool playClip;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (playClip)
                FadeCamera();
        }

        private void FadeCamera()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            var cameraFade = Game.Instance.CameraFade;
            if (mode == FadeCameraMode.FadeIn)
                cameraFade.FadeIn(clipDuration, method: method);
            else
                cameraFade.FadeOut(clipDuration, method: method);
        }
    }
}