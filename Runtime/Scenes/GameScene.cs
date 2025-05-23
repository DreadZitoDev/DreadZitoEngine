using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace DreadZitoEngine.Runtime.Scenes
{
    [Serializable]
    public class GameScene
    {
        public string Name;
        public LoadSceneMode Mode;

        public GameScene(string sceneName, LoadSceneMode mode)
        {
            Name = sceneName;
            Mode = mode;
        }

        public IEnumerator Load(Action onLoadedStatus = null)
        {
            // Load the scene
            var scene = SceneManager.LoadSceneAsync(Name, Mode);

            while (!scene.isDone)
                yield return null;

            onLoadedStatus?.Invoke();
        }
    }
}