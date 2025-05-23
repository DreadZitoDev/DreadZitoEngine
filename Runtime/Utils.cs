using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DreadZitoEngine.Runtime
{
    public static class GamePaths
    {
        public const string SCENES_FOLDER_PATH = "Assets/_DownfallProject/Scenes/";
        public const string ITEMS_DATA_PATH = "Data/ItemsData/";
    }
    
    public static class Utils
    {
        public static T ParseObject<T>(this object obj)
        {
            if (obj.GetType() != typeof(object) && obj.GetType() != typeof(JObject))
            {
                return (T) obj;
            }
            
            if (obj is JObject JObject)
            {
                return JObject.ToObject<T>();
            }
            return (T) obj;
        }
        
        public static T FindObjectByTypeInScene<T>(this Object unityObject, string sceneName, bool includeInactive = false) where T : Object
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
            {
                Debug.LogWarning($"Scene {sceneName} not found");
                return null;
            }

            return MonoBehaviour.FindObjectsOfType<T>(includeInactive).FirstOrDefault(e => (e as Component)?.gameObject.scene == scene);
        }

        public static T[] FindObjectsByTypeInScene<T>(this Object unityObject, string sceneName, bool includeInactive = false) where T : Object
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid())
            {
                Debug.LogWarning($"Scene {sceneName} not found");
                return null;
            }

            return MonoBehaviour.FindObjectsOfType<T>(includeInactive).Where(e => (e as Component)?.gameObject.scene == scene).ToArray();
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// EDITOR ONLY: Check if a scene contains a script of type T
        /// </summary>
        /// <param name="sceneName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool SceneContainsScript<T>(string sceneName)
        {
            // search scene path in build settings
            var path = EditorBuildSettings.scenes.FirstOrDefault(scene => scene.path.Contains(sceneName))?.path;
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning($"Scene {sceneName} not found in build settings");
                return false;
            }
            
            // Abre la escena en modo de solo lectura
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            bool containsScript = false;

            // Itera sobre los objetos de la escena
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                // Recorre todos los componentes de los objetos
                foreach (var component in obj.GetComponentsInChildren<Component>(true))
                {
                    if (component != null && component.GetType() == typeof(T))
                    {
                        containsScript = true;
                        break;
                    }
                }

                if (containsScript)
                    break;
            }

            // Cierra la escena después de inspeccionarla
            EditorSceneManager.CloseScene(scene, true);

            return containsScript;
        }
#endif
        
        public static float Vector3InverseLerp(Vector3 A, Vector3 B, Vector3 point, bool clamp = true)
        {
            Vector3 AB = B - A;
            float lengthSquared = AB.sqrMagnitude;

            // Handle case where A and B are the same point
            if (lengthSquared < Mathf.Epsilon) return 0f;

            Vector3 AP = point - A;
            float t = Vector3.Dot(AP, AB) / lengthSquared;

            if (clamp) t = Mathf.Clamp01(t);
        
            return t;
        }
    }
}