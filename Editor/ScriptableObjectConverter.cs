using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace DreadZitoEngine.Runtime.Editor
{
    public class ScriptableObjectConverter : EditorWindow
    {
        private ScriptableObject targetObject;
        public ScriptableObject[] targetObjects;
        private MonoScript newScript;

        [MenuItem("Tools/Convert ScriptableObject Type")]
        public static void ShowWindow()
        {
            GetWindow<ScriptableObjectConverter>("Convert SO Type");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Convert ScriptableObject", EditorStyles.boldLabel);

            targetObject = (ScriptableObject)EditorGUILayout.ObjectField("Target Asset", targetObject, typeof(ScriptableObject), false);
            newScript = (MonoScript)EditorGUILayout.ObjectField("New Script (child)", newScript, typeof(MonoScript), false);
            
            SerializedObject so = new SerializedObject(this);
            SerializedProperty arrayProperty = so.FindProperty("targetObjects");

            EditorGUILayout.PropertyField(arrayProperty, new GUIContent("Target Assets"), true);
            so.ApplyModifiedProperties();

            if (GUILayout.Button("Convert") && newScript != null)
            {
                foreach (var scriptable in targetObjects)
                {
                    ConvertScriptableObject(scriptable);
                }
            }
        }

        private void ConvertScriptableObject(ScriptableObject targetObj)
        {
            string path = AssetDatabase.GetAssetPath(targetObj);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Invalid asset path.");
                return;
            }

            string assetText = File.ReadAllText(path);

            // Buscamos el script GUID nuevo
            string newScriptPath = AssetDatabase.GetAssetPath(newScript);
            string newScriptGUID = AssetDatabase.AssetPathToGUID(newScriptPath);

            // Reemplazamos la línea m_Script
            string updatedText = System.Text.RegularExpressions.Regex.Replace(
                assetText,
                @"m_Script: {fileID: \d+, guid: [a-f0-9]+, type: \d+}",
                $"m_Script: {{fileID: 11500000, guid: {newScriptGUID}, type: 3}}"
            );

            File.WriteAllText(path, updatedText);
            AssetDatabase.ImportAsset(path);

            Debug.Log("Asset converted to new type.");
        }
    }

}