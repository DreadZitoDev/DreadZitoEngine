using UnityEditor;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Editor
{
    public class TimeScaleEditorWindow : EditorWindow
    {
        private SerializedObject serializedObject;
        private SerializedProperty stepProperty;

        private const float defaultTimeScale = 1f;
        private const float minStepValue = 0.1f;

        [MenuItem("Tools/Time Scale Controller™")]
        public static void ShowWindow()
        {
            GetWindow<TimeScaleEditorWindow>("Time Scale");
        }

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            stepProperty = serializedObject.FindProperty("step");
        }

        private void OnGUI()
        {
            serializedObject.Update();

            // Step configuration
            EditorGUILayout.LabelField("Step Configuration", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(stepProperty, new GUIContent("Step Value"));
            stepProperty.floatValue = Mathf.Max(stepProperty.floatValue, minStepValue);

            EditorGUILayout.Space(10);

            // Time scale controls
            EditorGUILayout.LabelField("Time Scale Controls", EditorStyles.boldLabel);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    Time.timeScale = Mathf.Max(0, Time.timeScale - stepProperty.floatValue);
                }

                EditorGUILayout.LabelField($"{Time.timeScale:F1}", GUILayout.Width(100));

                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    Time.timeScale += stepProperty.floatValue;
                }
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Reset to Default"))
            {
                Time.timeScale = defaultTimeScale;
            }

            serializedObject.ApplyModifiedProperties();
        }

        // Serialized field for step value
        [SerializeField] private float step = 1f;
    }
}