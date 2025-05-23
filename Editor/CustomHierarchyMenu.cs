using UnityEditor;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Editor
{
    public class CustomHierarchyMenu
    {
        public const string PLAYER_SPAWN_PREFAB_PATH = "Assets/DreadZitoEngine/Prefabs/PlayerSpawn";
        public const string TRIGGER_PREFAB_PATH = "Assets/DreadZitoEngine/Prefabs/Engine/Trigger";
        public const string HOTSPOT_PREFAB_PATH = "Assets/DreadZitoEngine/Prefabs/InteractionSystem/Hotspot";
        public const string CUTSCENE_CONTENT_PREFAB_PATH = "Assets/DreadZitoEngine/Prefabs/CutsceneContent";
        public const string GAME_PREFAB_PATH = "Assets/DreadZitoEngine/Prefabs/GAME";
        
        [MenuItem("GameObject/DreadZitoEngine/PlayerSpawn", false, 1)]
        static void CreatePlayerSpawn()
        {
            InstantiatePrefab(PLAYER_SPAWN_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/DreadZitoEngine/Hotspot", false, 1)]
        static void CreateHotspot()
        {
            InstantiatePrefab(HOTSPOT_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/DreadZitoEngine/Trigger", false, 1)]
        static void CreateTrigger()
        {
            InstantiatePrefab(TRIGGER_PREFAB_PATH);
        }

        [MenuItem("GameObject/DreadZitoEngine/CutsceneContent", false, 1)]
        static void CreateCutsceneContent()
        {
            InstantiatePrefab(CUTSCENE_CONTENT_PREFAB_PATH, false);
        }
        
        [MenuItem("GameObject/DreadZitoEngine/GAME", false, 1)]
        static void CreateGame()
        {
            InstantiatePrefab(GAME_PREFAB_PATH, false);
        }

        static GameObject InstantiatePrefab(string prefabPath, bool autoSelect = true)
        {
            // Cargar el prefab desde la carpeta Resources
            //GameObject prefab = Resources.Load<GameObject>(prefabPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + ".prefab");
        
            // Verificar si se ha encontrado el prefab
            if (prefab != null)
            {
                // Instanciar el prefab en la escena
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        
                // Si hay un objeto seleccionado en la jerarquía, lo asignamos como padre
                if (Selection.activeTransform != null)
                {
                    instance.transform.SetParent(Selection.activeTransform);
                }
        
                // Registrar la operación para el sistema de deshacer
                Undo.RegisterCreatedObjectUndo(instance, "Instantiate " + instance.name);
                if (autoSelect)
                    Selection.activeObject = instance;
            }
            else
            {
                Debug.LogError("Prefab not found in Resources/" + prefabPath + "!");
            }

            return prefab;
        }
    }
}