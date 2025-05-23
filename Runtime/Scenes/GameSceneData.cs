using DreadZitoTools.ScriptableLovers;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Scenes
{
    [CreateAssetMenu(fileName = "GameSceneData", menuName = "DownfallProject/GameSceneData", order = 0)]
    [ScriptableObjectPath("Assets/_Room502/Data/Scene")]
    public class GameSceneData : ScriptableObject
    {
        public string EnvironmentName;
        public string[] LogicSceneNames = new string[]{"Gameplay"};
    }
}