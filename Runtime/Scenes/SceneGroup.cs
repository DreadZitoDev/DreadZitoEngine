using UnityEngine;

namespace DreadZitoEngine.Runtime.Scenes
{
    [CreateAssetMenu(fileName = "GameSceneData", menuName = "DreadZitoEngine/GameSceneData", order = 0)]
    public class SceneGroup : ScriptableObject
    {
        public SceneReference EnvironmentRef;
        public SceneReference[] LogicSceneRefs;
    }
}