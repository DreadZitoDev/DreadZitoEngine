using System;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tags
{
    /// <summary>
    /// This class is used to reference a particular ID between assets scope and scenes
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectID", menuName = "DownfallProject/ObjectID", order = 0)]
    public class ObjectID : ScriptableObject
    {
        public string ID;
        
        public void RefreshID()
        {
            ID = Guid.NewGuid().ToString();
            // Save change in current scriptable object
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public GameObject FindInstanceInScene()
        {
            return Game.GetSceneObject(ID);
        }
        public T FindInstanceInScene<T>() where T : Component
        {
            return Game.GetSceneObject(ID)?.GetComponent<T>();
        }

        public override string ToString()
        {
            return $"[{name} - {ID}]";
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(ObjectID))]
    public class ObjectID_HolderSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var objId = (ObjectID) target;
            if (GUILayout.Button("Refresh ID"))
            {
                objId.RefreshID();
            }
        }
    }
    #endif
}