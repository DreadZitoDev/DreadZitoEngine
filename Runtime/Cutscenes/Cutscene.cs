using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Tags;
using DreadZitoEngine.Runtime;
using PixelCrushers.DialogueSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace DreadZitoEngine.Runtime.Cutscenes
{
    [Serializable]
    public class BindingData
    {
        public ObjectID_HolderSO objectID;
        public string key;
        public TrackAsset trackAsset;
    }
    
    public class Cutscene : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        
        //^ ensures the match starts at the beginning of the string
        // (?i): case insensitive
        // ( ?): Accept optional space
        // $ ensures the match ends at the end of the string
        private Regex CinemachineTrackBinding = new Regex(@"^(?i)Cinemachine ?Track$", RegexOptions.Compiled);
        private Regex PlayerTrackBinding = new Regex(@"^(?i)Player", RegexOptions.Compiled);
        
        public event Action<CutsceneData> OnCutsceneStarted;
        public event Action<CutsceneData> OnCutscenePaused;
        public event Action<CutsceneData> OnCutsceneFinished;

        private bool initialized;
        
        private CutsceneData data;

        [SerializeField] private List<BindingData> bindingDatas;

#if UNITY_EDITOR
        private void Start()
        {
            // Play test this cutscene if it's set to play on awake and the game is not running
            if (testPlayOnAwake && GameplayMain.Instance == null)
                PlayTestCutscene();
        }
#endif

        public void PlayCutscene(CutsceneData data)
        {
            if (!initialized)
                Initialize(data);
            
            director.Play();
        }

        private void Initialize(CutsceneData data)
        {
            this.data = data;
            
            director.played += CutscenePlayed;
            director.paused += CutscenePaused;
            director.stopped += CutsceneFinished;
            
            ProcessRuntimeBindings();
            
            initialized = true;
        }
        
        private void CutscenePlayed(PlayableDirector obj)
        {
            Debug.Log($"Cutscene Started");
            OnCutsceneStarted?.Invoke(data);
        }

        private void CutscenePaused(PlayableDirector director)
        {
            Debug.Log($"Cutscene Paused");
            OnCutscenePaused?.Invoke(data);
        }

        private void CutsceneFinished(PlayableDirector director)
        {
            if (data != null)
            {
                foreach (var dialogueActor in this.FindObjectsByTypeInScene<DialogueActor>(data?.CutsceneSceneName)) {
                    dialogueActor.gameObject.SetActive(true);
                }
            }
            else
            {
                Debug.LogWarning($"Cutscene data is null, unable to find dialogue actors");
            }
            
            
            Debug.Log($"Cutscene Ended");
            OnCutsceneFinished?.Invoke(data);
        }

        public void UpdateBindings()
        {
            var bindings = director.playableAsset.outputs;
            
            // Clear any null track assets
            var tmpList = new List<BindingData>(bindingDatas);
            foreach (var data in tmpList)
            {
                if (data.trackAsset == null)
                {
                    bindingDatas.Remove(data);
                }
            }
            
            // Add new bindings
            foreach (var playableBinding in bindings)
            {
                // check if track is not binding to a game object
                var trackAsset = playableBinding.sourceObject as TrackAsset;
                if (bindingDatas.Exists(x => x.trackAsset == trackAsset))
                    continue;
                
                bindingDatas.Add(new BindingData()
                {
                    key = playableBinding.streamName,
                    trackAsset = trackAsset
                });
            }
        }
        
        // Signal called from timeline
        public void TeleportPlayer(Transform targetPos)
        {
            var player = GameplayMain.Instance?.Player;
            if (player == null)
                return;
            player.TeleportTo(targetPos);
        }
        
        private void ProcessRuntimeBindings()
        {
            foreach (var bindInfo in bindingDatas)
            {
                ProcessBindingData(bindInfo);
            }
        }

        private void ProcessBindingData(BindingData bindInfo)
        {
            if (bindInfo.objectID != null)
            {
                var go = bindInfo.objectID.FindInstanceInScene();
                if (go != null)
                {
                    director.SetGenericBinding(bindInfo.trackAsset, go);
                }

                return;
            }
            
            if (PlayerTrackBinding.Match(bindInfo.key).Success)
            {
                var player = GameplayMain.Instance.Player;
                director.SetGenericBinding(bindInfo.trackAsset,  bindInfo.trackAsset.GetType().Name switch
                {
                    nameof(AnimationTrack) => player.Animator,
                    _ => player.gameObject
                });
            }

            if (CinemachineTrackBinding.Match(bindInfo.key).Success)
            {
                var brain = CinemachineBrain.GetActiveBrain(0);
                director.SetGenericBinding(bindInfo.trackAsset, brain);
            }
        }

        // TESTS
        [Header("DEBUGGING")]
        [SerializeField] private CutsceneData testCutsceneData;
        [SerializeField] private bool testPlayOnAwake;
        [ContextMenu("PlayTestCutscene")]
        public void PlayTestCutscene()
        {
            PlayCutscene(testCutsceneData);
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Cutscene))]
    public class CutsceneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Cutscene cutscene = (Cutscene) target;
            
            DrawDefaultInspector();
            
            GUILayout.Space(10);        
                
            if (GUILayout.Button("Update Bindings"))
            {
                cutscene.UpdateBindings();
            }
        }
    }
    #endif
}