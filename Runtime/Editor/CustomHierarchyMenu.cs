using UnityEditor;
using UnityEngine;

namespace DreadZitoEngine.Runtime.Editor
{
    public class CustomHierarchyMenu
    {
        
        [MenuItem("GameObject/ROOM 502/PlayerSpawn", false, 1)]
        static void CreatePlayerSpawn()
        {
            InstantiatePrefab(CustomEditorsDefinitions.PLAYER_SPAWN_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/ROOM 502/Hotspot", false, 1)]
        static void CreateHotspot()
        {
            InstantiatePrefab(CustomEditorsDefinitions.HOTSPOT_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/ROOM 502/Interactable", false, 1)]
        static void CreateInteractable()
        {
            InstantiatePrefab(CustomEditorsDefinitions.INTERACTABLE_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/ROOM 502/Trigger", false, 1)]
        static void CreateTrigger()
        {
            InstantiatePrefab(CustomEditorsDefinitions.TRIGGER_PREFAB_PATH);
        }
        
        [MenuItem("GameObject/ROOM 502/EventSystem", false, 1)]
        static void CreateEventSystem()
        {
            InstantiatePrefab(CustomEditorsDefinitions.EVENT_SYSTEM_PREFAB_PATH);
        }

        [MenuItem("GameObject/ROOM 502/CutsceneContent", false, 1)]
        static void CreateCutsceneContent()
        {
            InstantiatePrefab(CustomEditorsDefinitions.CUTSCENE_CONTENT_PREFAB_PATH, false);
        }
        
        [MenuItem("GameObject/ROOM 502/GAME", false, 1)]
        static void CreateGame()
        {
            InstantiatePrefab(CustomEditorsDefinitions.GAME_PREFAB_PATH, false);
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