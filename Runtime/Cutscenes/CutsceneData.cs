using DreadZitoEngine.Runtime.CameraCode;
using DreadZitoEngine.Runtime.Scenes;
using DreadZitoTools.ScriptableLovers;
using FlowCanvas;
using UnityEditor;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Cutscenes
{
    [CreateAssetMenu(fileName = "CutsceneData", menuName = "DownfallProject/CutscenesData", order = 0)]
    [ScriptableObjectPath("Assets/_DownfallProject/Resources/Data/Cutscenes")]
    public class CutsceneData : ScriptableObject
    {
        public SceneReference CutsceneScene;
        public bool disablePlayerMovement = true;
        public bool hidePlayerVisibility = true;
        public bool restorePlayerVisibility = true;
        public bool FadeCameraWhenLoading = true;
        public bool isSkippable;

        public FadeMethod fadeMethodWhenLoading = FadeMethod.OnGUI;
        
        [Tooltip("FlowScript to load after the cutscene is finished")]
        public FlowScript FlowScript;
        
        public string CutsceneSceneName => CutsceneScene.SceneName;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CutsceneData))]
    public class CutscenesDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CutsceneData cutscene = (CutsceneData) target;
            
            var sceneData = cutscene.CutsceneScene;
            if (sceneData != null)
            {
                if (!Application.isPlaying)
                {
                    var anySceneContainsCutscene = false;
                    
                    anySceneContainsCutscene = Utils.SceneContainsScript<Cutscene>(sceneData.SceneName);
                    if (!anySceneContainsCutscene)
                    {
                        EditorGUILayout.HelpBox($"Scene {sceneData.SceneName} does not contain Cutscene script", MessageType.Error);
                    }
                }
            }
            
            // Draw all other properties, ignore "m_script"
            DrawPropertiesExcluding(serializedObject, new[] { "m_Script" });
            
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}