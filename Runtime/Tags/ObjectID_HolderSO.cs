using System;
using _Room502.Scripts;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Tags
{
    /// <summary>
    /// This class is used to reference a particular ID between assets scope and scenes
    /// </summary>
    [CreateAssetMenu(fileName = "ObjectID_Holder", menuName = "DownfallProject/ObjectID_Holder", order = 0)]
    public class ObjectID_HolderSO : ScriptableObject
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
    [UnityEditor.CustomEditor(typeof(ObjectID_HolderSO))]
    public class ObjectID_HolderSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var objId = (ObjectID_HolderSO) target;
            if (GUILayout.Button("Refresh ID"))
            {
                objId.RefreshID();
            }
        }
    }
    #endif
}