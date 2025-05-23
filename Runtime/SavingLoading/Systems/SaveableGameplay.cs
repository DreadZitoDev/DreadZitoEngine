using System;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.Gameplay;
using UnityEngine;

namespace DreadZitoEngine.Runtime.SavingLoading.Systems
{
    [Serializable]
    public class GameplayData
    {
        public object PlayerData;
    }
    
    public class SaveableGameplay : MonoBehaviour, ISaveable
    {
        public object CaptureState()
        {
            var gameplay = GameplayMain.Instance;
            var saveablePlayer = gameplay.Player.GetComponent<ISaveable>();
            
            return new GameplayData
            {
                PlayerData = saveablePlayer.CaptureState(),
            };
        }

        public void RestoreState(object state, Action onLoadComplete = null)
        {
            var gameplayData = state.ParseObject<GameplayData>();
            
            var gameplay = GameplayMain.Instance;
            var saveablePlayer = gameplay.Player.GetComponent<ISaveable>();
            saveablePlayer.RestoreState(gameplayData.PlayerData, onLoadComplete);
            
            
        }
    }
}