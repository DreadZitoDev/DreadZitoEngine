using DreadZitoEngine.Runtime.Tags;

namespace DreadZitoEngine.Runtime.Editor
{
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class ObjectIDInjector
    {
        static ObjectIDInjector()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }

        static void OnPostHeaderGUI(Editor editor)
        {
            // Solo para GameObjects en escena
            if (editor.target is GameObject go)
            {
                // Verificá que no tenga ya el componente
                if (go.GetComponent<SceneObjectID>() == null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(40); // Padding para alinear con el header
                    if (GUILayout.Button("➕ Add SceneObjectID", GUILayout.Height(20)))
                    {
                        go.AddComponent<SceneObjectID>();
                        EditorUtility.SetDirty(go);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
    }

}