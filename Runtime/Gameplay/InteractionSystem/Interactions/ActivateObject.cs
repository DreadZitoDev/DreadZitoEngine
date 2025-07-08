using System.Collections;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class ActivateObject : HotspotInteractionBase
    {
        [Space]
        [SerializeField] private bool setActiveState = true;
        [SerializeField] private GameObject target;
        [SerializeField] private ObjectID targetID;
        [Space]
        [SerializeField] private string animationName;

        protected override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var targetObject = target != null ? target : Game.GetSceneObject(targetID);
            if (targetObject == null) {
                Debug.LogError("Target object not found for ActivateObject interaction");
                yield break;
            }
            
            targetObject.SetActive(setActiveState);
            yield return base.DoInteraction(hotspot);
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ActivateObject))]
    public class ActivateObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var interaction = (ActivateObject)target;
            SerializedProperty targetObj = serializedObject.FindProperty("target");
            SerializedProperty targetObjID = serializedObject.FindProperty("targetID");
            
            DrawPropertiesExcluding(serializedObject, "target", "targetID");
            
            var animatorValue = targetObj.objectReferenceValue;
            var animatorIDValue = targetObjID.objectReferenceValue;
            
            if (animatorValue == null && animatorIDValue == null)
            {
                EditorGUILayout.PropertyField(targetObj);
                EditorGUILayout.PropertyField(targetObjID);
            }
            else if (animatorValue != null)
            {
                EditorGUILayout.PropertyField(targetObj);
                targetObjID.objectReferenceValue = null; // Clear animatorID if animator is set
            }
            else if (animatorIDValue != null)
            {
                EditorGUILayout.PropertyField(targetObjID);
                targetObj.objectReferenceValue = null; // Clear animator if animatorID is set
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}