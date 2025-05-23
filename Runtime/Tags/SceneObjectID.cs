using System;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tags
{
    /// <summary>
    /// Scene object ID
    /// </summary>
    public class SceneObjectID : MonoBehaviour
    {
        [SerializeField, Tooltip("If is not set, this GameObject will be returned")]
        private GameObject sceneObject;

        [SerializeField] private ObjectID idHolder;
        [SerializeField] private string id;
        
        // If idHolder is set, ID will be taken from it
        public string ID => idHolder != null ? idHolder.ID : id;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                RefreshID();
            }
        }
        
        public void RefreshID()
        {
            id = Guid.NewGuid().ToString();
            // Save change in current prefab
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
        }
        
        public GameObject GetSceneObject()
        {
            return sceneObject != null ? sceneObject : gameObject;
        }
    }
    
    #region Editor
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SceneObjectID))]
    public class ObjectIDEditor : UnityEditor.Editor
    {
        private UnityEditor.SerializedProperty id;
        private UnityEditor.SerializedProperty idHolder;
        private UnityEditor.SerializedProperty sceneObject;

        private void OnEnable()
        {
            id = serializedObject.FindProperty("id");
            idHolder = serializedObject.FindProperty("idHolder");
            sceneObject = serializedObject.FindProperty("sceneObject");
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            serializedObject.Update();
            
            UnityEditor.EditorGUILayout.LabelField("If is not set, this GameObject will be returned", UnityEditor.EditorStyles.boldLabel);
            UnityEditor.EditorGUILayout.PropertyField(sceneObject);
            UnityEditor.EditorGUILayout.Space(5);
            
            if (idHolder.objectReferenceValue == null)
            {
                UnityEditor.EditorGUILayout.PropertyField(idHolder);
                UnityEditor.EditorGUILayout.PropertyField(id);
                UnityEditor.EditorGUILayout.Space();
                
                var objId = (SceneObjectID) target;
                if (GUILayout.Button("Refresh ID"))
                {
                    objId.RefreshID();
                }
            }
            else
            {
                UnityEditor.EditorGUILayout.PropertyField(idHolder);
                if (idHolder.objectReferenceValue != null)
                    UnityEditor.EditorGUILayout.LabelField("ID in asset scope", ((ObjectID)idHolder.objectReferenceValue).ID);
            }
            
            // write back serialized values to the real instance
            // automatically handles all marking dirty and undo/redo
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
    #endregion
}