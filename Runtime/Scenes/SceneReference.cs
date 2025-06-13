using UnityEngine;

namespace DreadZitoEngine.Runtime.Scenes
{
    [CreateAssetMenu(fileName = "SceneData", menuName = "DreadZitoEngine/SceneReference", order = 0)]
    public class SceneReference : ScriptableObject
    {
        public string SceneName;
    }
}