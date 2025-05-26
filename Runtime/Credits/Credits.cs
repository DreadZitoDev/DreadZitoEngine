using DreadZitoEngine.Runtime.CameraCode;
using DreadZitoEngine.Runtime.Cutscenes;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Credits
{
   
    public class Credits : MonoBehaviour
    {
        [SerializeField] private CutsceneData creditsCutscene;
 
        [Header("Debugs")]
        public bool debugPlayCutscene = true;
        
        private void Start()
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (debugPlayCutscene)
            #endif
            Game.Instance.CutsceneSystem.PlayCutscene(creditsCutscene);
            Game.Instance.CameraFade.FadeIn(0, method: FadeMethod.PostProcess);
        }
    }
}