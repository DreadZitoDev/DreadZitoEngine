using System.Collections;
using DreadZitoEngine.Runtime.Tags;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem.Interactions
{
    public class PlayAnimationInteraction : HotspotInteractionBase
    {
        [SerializeField] private Animator animator;
        [SerializeField] private ObjectID animatorID;
        [Space]
        [SerializeField] private string animationName;

        protected override IEnumerator DoInteraction(Hotspot hotspot)
        {
            var targetAnim = animator != null ? animator : Game.GetSceneObject<Animator>(animatorID);
            if (targetAnim == null)
            {
                Debug.LogError("Animator not found for PlayAnimationInteraction");
                yield break;
            }
            targetAnim.Play(animationName);
            yield return base.DoInteraction(hotspot);
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(PlayAnimationInteraction))]
    public class PlayAnimationInteractionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var interaction = (PlayAnimationInteraction)target;
            SerializedProperty animator = serializedObject.FindProperty("animator");
            SerializedProperty animatorID = serializedObject.FindProperty("animatorID");
            
            DrawPropertiesExcluding(serializedObject, "animator", "animatorID");
            
            var animatorValue = animator.objectReferenceValue;
            var animatorIDValue = animatorID.objectReferenceValue;
            
            if (animatorValue == null && animatorIDValue == null)
            {
                EditorGUILayout.PropertyField(animator);
                EditorGUILayout.PropertyField(animatorID);
            }
            else if (animatorValue != null)
            {
                EditorGUILayout.PropertyField(animator);
                animatorID.objectReferenceValue = null; // Clear animatorID if animator is set
            }
            else if (animatorIDValue != null)
            {
                EditorGUILayout.PropertyField(animatorID);
                animator.objectReferenceValue = null; // Clear animator if animatorID is set
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}