using System;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Gameplay.Players;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Cutscenes
{
    public class CutsceneInfo
    {
        public CutsceneData CutsceneData;
        public Cutscene Cutscene;
    }
    
    public class CutsceneSystem : MonoBehaviour
    {
        private CutsceneInfo currentCutscene;
        
        public bool IsPlayingCutscene => currentCutscene != null;
        public bool CurrentCutsceneBlocksMovement => IsPlayingCutscene && currentCutscene.CutsceneData.disablePlayerMovement;
        
        public event Action<CutsceneData> OnCutsceneStart;
        public event Action<CutsceneData> OnCutsceneFinished; 

        public void PlayCutscene(CutsceneData cutsceneData)
        {
            if (currentCutscene != null)
            {
                // TODO: INTERRUPT CURRENT CUTSCENE
                return;
            }
            
            if (cutsceneData == null || cutsceneData.CutsceneScene == null) {
                Debug.LogError($"Cutscene {cutsceneData} is null or does not have a scene assigned");
                return;
            }

            var player = GameplayMain.Instance?.Player;
            var fadeCamera = cutsceneData.FadeCameraWhenLoading;
            
            currentCutscene = new CutsceneInfo();
            currentCutscene.CutsceneData = cutsceneData;
            
            if (cutsceneData.disablePlayerMovement)
            {
                player?.PlayerLockMovement(Player.CUTSCENE_MOVE_BLOCKER_ID, true);
            }

            
            
            Game.Instance.LoadScene(cutsceneData.CutsceneScene, fadeCamera: fadeCamera,
                onLoadComplete: () =>
                {
                    if (fadeCamera) return;
                    if (cutsceneData.hidePlayerVisibility)
                        player?.SetModelVisibility(false);
                    StartCutscene(cutsceneData);
                    
                },  onCameraFadedIn: () =>
                {
                    if (cutsceneData.hidePlayerVisibility)
                        player?.SetModelVisibility(false);
                }, 
                onPreCameraFadedOut: () =>
                {
                    if (fadeCamera)
                        StartCutscene(cutsceneData);
                });
        }

        private void StartCutscene(CutsceneData cutsceneData)
        {
            var cutsceneSceneLogic = cutsceneData.CutsceneSceneName;
            var cutscene = this.FindObjectByTypeInScene<Cutscene>(cutsceneSceneLogic);
                
            currentCutscene.Cutscene = cutscene;
                
            cutscene.OnCutsceneFinished += CutsceneFinished;
            cutscene.OnCutsceneStarted += CutsceneStarted;
            cutscene.PlayCutscene(cutsceneData);
        }
        
        private void CutsceneStarted(CutsceneData cutsceneData)
        {
            OnCutsceneStart?.Invoke(cutsceneData);
        }

        private void CutsceneFinished(CutsceneData cutsceneData)
        {
            var player = GameplayMain.Instance?.Player;
            
            if (cutsceneData.hidePlayerVisibility && cutsceneData.restorePlayerVisibility)
            {
                player?.SetModelVisibility(true);
            }
            
            if (cutsceneData.disablePlayerMovement)
            {
                player?.RemoveMoveBlocker(Player.CUTSCENE_MOVE_BLOCKER_ID);
            }
            
            currentCutscene = null;
            OnCutsceneFinished?.Invoke(cutsceneData);

            Game.Instance.UnLoadScene(cutsceneData.CutsceneScene, true);
            
            if (cutsceneData.FlowScript != null)
            {
                Game.Instance.RunFlowScript(cutsceneData.FlowScript);
            }
        }
    }
}