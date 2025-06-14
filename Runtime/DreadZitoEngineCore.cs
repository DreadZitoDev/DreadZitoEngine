using DreadZitoEngine.Runtime.Gameplay;
using DreadZitoEngine.Runtime.Gameplay.Players;
using UnityEngine;

namespace DreadZitoEngine.Runtime
{
    public static class DreadZitoEngineCore
    {
        public static GameplayMain GameplayInstance => GameplayMain.Instance;
        public static Game GameInstance => Runtime.Game.Instance;
        public static Player PlayerInstance => GameplayInstance?.Player;

        public static T Gameplay<T>() where T : GameplayMain
        {
            if (GameplayInstance is T g) return g;
            Debug.LogError($"GameplayMain is not of type {typeof(T).Name}");
            return null;
        }

        public static T Game<T>() where T : Game
        {
            if (GameInstance is T g) return g;
            Debug.LogError($"Game is not of type {typeof(T).Name}");
            return null;
        }

        public static T Player<T>() where T : Player
        {
            if (PlayerInstance is T p) return p;
            Debug.LogError($"Player is not of type {typeof(T).Name}");
            return null;
        }
    }

}