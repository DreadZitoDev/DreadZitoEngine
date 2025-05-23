using System;
using System.Collections;
using System.IO;
using _Room502.Scripts;
using DreadZitoEngine.Runtime.SavingLoading.Systems;
using Newtonsoft.Json;
using UnityEngine;

namespace DreadZitoEngine.Runtime.SavingLoading
{
    [Serializable]
    public class SaveFileData
    {
        public string EnvironmentScene;
        public object GameplayData;
        public object QuestsManagerData;
    }
    
    public class SaveSystem : MonoBehaviour
    {
        private string SAVE_PATH => $"{Application.persistentDataPath}/SaveFile.json";
        

        //TODO: Determine how to call this methods across the game 
        public void Save()
        {
            var state = LoadFile();
            CaptureGameState(state);
            SaveFile(state);
        }

        private void CaptureGameState(SaveFileData state)
        {
            // Capture the whole game state
            // Capture Order: Gameplay (environment, player, etc) -> QuestsManager
            var saveableGameplay = FindAnyObjectByType<SaveableGameplay>();
            var saveableQuestsManager = FindAnyObjectByType<SaveableQuestsManager>();

            state.EnvironmentScene = Game.Instance.CurrentEnvironment;
            state.GameplayData = saveableGameplay.CaptureState();
            state.QuestsManagerData = saveableQuestsManager.CaptureState();
        }

        private void SaveFile(SaveFileData state)
        {
            using (var stream = File.Open(SAVE_PATH, FileMode.Create))
            {
                var jsonText = JsonConvert.SerializeObject(state);
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonText);
                stream.Write(bytes, 0, bytes.Length);
            }
        }
        
        public void Load()
        {
            Game.Instance.ResetSystems();
            var state = LoadFile();
            
            // Restore the whole game state
            // Load Order: Gameplay (environment, player, etc) -> QuestsManager
            StartCoroutine(LoadGameRoutine(state));
        }
        
        private SaveFileData LoadFile()
        {
            var saveFileData = new SaveFileData();
            if (!File.Exists(SAVE_PATH))
            {
                return saveFileData;
            }

            using (var stream = File.Open(SAVE_PATH, FileMode.Open))
            {
                var reader = new StreamReader(stream);
                var jsonText = reader.ReadToEnd();
                saveFileData = JsonConvert.DeserializeObject<SaveFileData>(jsonText);
                // If is null create a new instance
                saveFileData ??= new SaveFileData();
                
                return saveFileData;
            }
        }

        IEnumerator LoadGameRoutine(SaveFileData state)
        {
            var loadedGameplay = false;
            
            // Load gameplay
            Game.Instance.LoadGameplayInEnvironment(state.EnvironmentScene, () =>
            {
                var saveableGameplay = FindAnyObjectByType<SaveableGameplay>();
                
                saveableGameplay.RestoreState(state.GameplayData);
                
                loadedGameplay = true;
            });

            yield return new WaitUntil(() => loadedGameplay);
            
            // Load quests state
            var questsLoaded = false;
            var saveableQuestsManager = FindAnyObjectByType<SaveableQuestsManager>();
            saveableQuestsManager.RestoreState(state.QuestsManagerData, () =>
            {
                questsLoaded = true;
            });

            yield return new WaitUntil(() => questsLoaded);
            
            // Load interaction system state
            var interactionSystemLoaded = false;
            
            Debug.Log("Game loaded successfully!");
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(SaveSystem))]
    public class SaveSystemEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var saveSystem = (SaveSystem) target;
            GUILayout.Space(20);
            if (GUILayout.Button("Save"))
            {
                saveSystem.Save();
            }
            if (GUILayout.Button("Load"))
            {
                saveSystem.Load();
            }
        }
    }
    #endif
}