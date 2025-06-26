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

        public bool IsGroupScene;
        public SceneGroup SceneGroup;
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(CutsceneData))]
    public class CutscenesDataEditor : UnityEditor.Editor
    {
       private string lastSceneName;
    private bool sceneIsValid;

    private void OnEnable()
    {
        ValidateCutsceneScene(); // Validación inicial al abrir el inspector
    }

    public override void OnInspectorGUI()
    {
        CutsceneData cutscene = (CutsceneData)target;

        serializedObject.Update();

        SerializedProperty isGroupSceneProp = serializedObject.FindProperty("IsGroupScene");
        SerializedProperty sceneGroupProp = serializedObject.FindProperty("SceneGroup");
        SerializedProperty cutsceneSceneProp = serializedObject.FindProperty("CutsceneScene");

        // Detectar cambio en CutsceneScene
        string currentSceneName = cutscene.CutsceneScene?.SceneName;
        if (currentSceneName != lastSceneName)
        {
            lastSceneName = currentSceneName;
            ValidateCutsceneScene();
        }

        // Mostrar error si es inválida
        if (!sceneIsValid && !Application.isPlaying && !string.IsNullOrEmpty(lastSceneName))
        {
            EditorGUILayout.HelpBox($"Scene {lastSceneName} does not contain Cutscene script", MessageType.Error);
        }

        // Mostrar todo menos los campos que manejamos a mano
        DrawPropertiesExcluding(serializedObject, new[] { "m_Script", "IsGroupScene", "SceneGroup" });

        // Mostrar IsGroupScene y SceneGroup con lógica de visibilidad
        EditorGUILayout.PropertyField(isGroupSceneProp);
        if (isGroupSceneProp.boolValue)
        {
            EditorGUILayout.PropertyField(sceneGroupProp);
        }
        else if (sceneGroupProp.objectReferenceValue != null)
        {
            sceneGroupProp.objectReferenceValue = null;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ValidateCutsceneScene()
    {
        CutsceneData cutscene = (CutsceneData)target;
        string sceneName = cutscene.CutsceneScene?.SceneName;

        if (!string.IsNullOrEmpty(sceneName) && !Application.isPlaying)
        {
            sceneIsValid = Utils.SceneContainsScript<Cutscene>(sceneName);
        }
        else
        {
            sceneIsValid = true; // No escena = no error
        }
    }
}
#endif
}