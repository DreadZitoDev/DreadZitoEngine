using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DreadZitoEngine.Runtime.Audio;
using DreadZitoEngine.Runtime.CameraCode;
using DreadZitoEngine.Runtime.Cutscenes;
using DreadZitoEngine.Runtime.Dialogue;
using DreadZitoEngine.Runtime.QuestManager;
using DreadZitoEngine.Runtime.SavingLoading;
using DreadZitoEngine.Runtime.Scenes;
using DreadZitoEngine.Runtime.Tags;
using FlowCanvas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Room502.Scripts
{ 
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameSceneData mainMenuScene;
        [SerializeField] private FlowScript newGameFlowScript;
        [SerializeField] private GameSceneData creditsScene;

        public static Game Instance { get; private set; }

        public QuestsSystem QuestsSystem { get; private set; }
        public string CurrentEnvironment => SceneManager.GetActiveScene().name;

        private CameraFade cameraFade;
        public CameraFade CameraFade => cameraFade ? cameraFade : cameraFade = FindObjectOfType<CameraFade>();
        
        public CutsceneSystem CutsceneSystem { get; private set; }
        public DialogueSystemHandler DialogueSystemHandler { get; private set; }
        public SaveSystem SaveSystem { get; private set; }
        public BgMusic BgMusic { get; private set; }
        
        public event Action<string> OnSceneLoaded; 
        public event Action<string, GameSceneData> OnSceneUnloaded;

        [SerializeField, Tooltip("Base prefab for running FLowScripts")]
        private FlowScriptController flowScriptControllerPrefab;

        private Dictionary<Coroutine, FlowScriptController> RunningFlowScripts = new();
        
        public event Action<GameSceneData> OnPrepareSwitchingEnvironment;
        public event Action<GameSceneData> OnSwitchEnvironment;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            CutsceneSystem = GetComponentInChildren<CutsceneSystem>();
            QuestsSystem = GetComponentInChildren<QuestsSystem>();
            DialogueSystemHandler = GetComponentInChildren<DialogueSystemHandler>();
            SaveSystem = GetComponentInChildren<SaveSystem>();
            BgMusic = GetComponentInChildren<BgMusic>();
            SceneManager.sceneLoaded += SceneManagerLoaded;
            
            if (SceneManager.GetActiveScene().name == "_Launcher")
            {
                LoadTitleScreen();
            }
        }

        private Coroutine loadSceneCoroutine;
        
        public void LoadTitleScreen()
        {
            LoadScene(mainMenuScene, fadeCamera: true);
        }
        
        public void LoadScene(GameSceneData sceneData, Action onLoadComplete = null, bool fadeCamera = false, Action onCameraFadedIn = null, Action onPreCameraFadedOut = null, FadeMethod fadeMethod = FadeMethod.OnGUI)
        {
           if (loadSceneCoroutine != null)
                StopCoroutine(loadSceneCoroutine);
           loadSceneCoroutine = StartCoroutine(LoadScenes(sceneData.EnvironmentName, sceneData.LogicSceneNames, onLoadComplete, fadeCamera, onCameraFadedIn, onPreCameraFadedOut, fadeMethod));
        }
        
        public void LoadGameplayInEnvironment(string environment, Action onLoadComplete = null, bool fadeCamera = false)
        {
            if (loadSceneCoroutine != null)
                StopCoroutine(loadSceneCoroutine);
            loadSceneCoroutine = StartCoroutine(LoadScenes(environment, new []{"Gameplay"}, onLoadComplete, fadeCamera));
        }

        private IEnumerator LoadScenes(string environment = null, string[] logicScenes = null,
            Action onLoadComplete = null, bool fadeCamera = false, Action onCameraFadedIn = null, Action onPreCameraFadedOut = null, FadeMethod fadeMethod = FadeMethod.OnGUI)
        {
            // TODO: Probably not the best part to put this, but we need to speed run it
            if (fadeCamera)
            {
                // avoid fading in if the camera is already faded in
                yield return CameraFade.FadeInCameraRoutine(method: fadeMethod);
                onCameraFadedIn?.Invoke();
            }
            
            GameScene environmentScene = null;
            if (!string.IsNullOrEmpty(environment))
                environmentScene = new GameScene(environment, LoadSceneMode.Single);

            if ((environmentScene == null && logicScenes == null)) {
                Debug.LogWarning($"No scenes to load. You're so funny!");
                yield break;
            }
            
            if (environmentScene != null)
                yield return environmentScene.Load();
            
            if (logicScenes != null)
            {
                foreach (var logicScene in logicScenes)
                {
                    if (!string.IsNullOrEmpty(logicScene))
                    {
                        var logicSceneObj = new GameScene(logicScene, LoadSceneMode.Additive);
                        yield return logicSceneObj.Load();
                    }
                }
            }

            yield return null;

            if (fadeCamera)
            {
                onPreCameraFadedOut?.Invoke();
                yield return CameraFade.FadeOutCameraRoutine(method: fadeMethod);
            }
            
            onLoadComplete?.Invoke();
        }
        
        /// <summary>
        /// Reset all systems to their initial state, avoid having static inconsistent data
        /// </summary>
        public void ResetSystems()
        {
            QuestsSystem.Reset();
        }

        public void LoadEnvironment(GameSceneData sceneData, Action onLoadComplete = null, bool fadeCamera = false)
        {
            if (loadSceneCoroutine != null)
                StopCoroutine(loadSceneCoroutine);
            loadSceneCoroutine = StartCoroutine(LoadEnvironmentRoutine(sceneData, onLoadComplete, fadeCamera));
        }

        private IEnumerator LoadEnvironmentRoutine(GameSceneData environmentScene, Action onLoadComplete, bool fadeCamera)
        {
            OnPrepareSwitchingEnvironment?.Invoke(environmentScene);
            if (fadeCamera)
            {
                yield return CameraFade.FadeInCameraRoutine();
            }
            
            var currentEnvironmentScene = SceneManager.GetActiveScene();
            var currentEnvUnloadProcess = SceneManager.UnloadSceneAsync(currentEnvironmentScene);
            yield return new WaitUntil(() => currentEnvUnloadProcess.isDone);
            
            var nextEnvLoadProcess = SceneManager.LoadSceneAsync(environmentScene.EnvironmentName, LoadSceneMode.Additive);
            yield return new WaitUntil(() => nextEnvLoadProcess.isDone);
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(environmentScene.EnvironmentName));
            Debug.Log($"Environment loaded: {environmentScene.EnvironmentName}");
            
            if (fadeCamera)
                yield return CameraFade.FadeOutCameraRoutine();
            
            onLoadComplete?.Invoke();
            OnSwitchEnvironment?.Invoke(environmentScene);
        }
        
        public static GameObject GetSceneObject(string id)
        {
            var sceneObject = FindObjectsOfType<SceneObjectID>(true).FirstOrDefault(e => e.ID == id);
            return sceneObject == null ? null : sceneObject.GetSceneObject();
        }

        public bool IsLoaded(GameSceneData questScene)
        {
            if (!string.IsNullOrEmpty(questScene.EnvironmentName))
            {
                return SceneManager.GetSceneByName(questScene.EnvironmentName).isLoaded;
            }
            
            return questScene.LogicSceneNames.Any(sceneName => SceneManager.GetSceneByName(sceneName).isLoaded);
        }
        
        [Serializable]
        private class GameScene
        {
            public string Name;
            public LoadSceneMode Mode;

            public GameScene(string sceneName, LoadSceneMode mode)
            {
                Name = sceneName;
                Mode = mode;
            }

            public IEnumerator Load(Action onLoadedStatus = null)
            {
                // Load the scene
                var scene = SceneManager.LoadSceneAsync(Name, Mode);

                while (!scene.isDone)
                    yield return null;

                onLoadedStatus?.Invoke();
            }
        }

        public void RunFlowScript(FlowScript flowScript, Action onFinish = null)
        {
            var controller = Instantiate(flowScriptControllerPrefab, transform);
            var coroutine = default(Coroutine);
            coroutine = StartCoroutine(RunFlowScriptRoutine(controller, flowScript, () =>
            {
                onFinish?.Invoke();
                RunningFlowScripts.Remove(coroutine);
                Destroy(controller.gameObject);
            }));
            RunningFlowScripts.Add(coroutine, controller);
        }
        
        IEnumerator RunFlowScriptRoutine(FlowScriptController controller, FlowScript flowScript, Action onFinish)
        {
            controller.behaviour = flowScript;
            controller.StartBehaviour();
            yield return new WaitUntil(() => !controller.isRunning);

            Debug.Log($"Behaviour {flowScript.name} finished");
            onFinish?.Invoke();
        }

        public void NewGame()
        {
            RunFlowScript(newGameFlowScript);
        }

        public void UnLoadScene(GameSceneData sceneData, bool includeLogic = false)
        {
            StartCoroutine(UnLoadSceneRoutine(sceneData, includeLogic));
        }
        
        IEnumerator UnLoadSceneRoutine(GameSceneData sceneData, bool includeLogic)
        {
            var environmentScene = sceneData.EnvironmentName;
            if (!string.IsNullOrEmpty(environmentScene))
            {
                var unloadProcess = SceneManager.UnloadSceneAsync(environmentScene);
                yield return new WaitUntil(() => unloadProcess.isDone);
                OnSceneUnloaded?.Invoke(environmentScene, sceneData);
            }
            
            if (includeLogic)
            {
                foreach (var logicScene in sceneData.LogicSceneNames)
                {
                    var unloadProcess = SceneManager.UnloadSceneAsync(logicScene);
                    yield return new WaitUntil(() => unloadProcess.isDone);
                    OnSceneUnloaded?.Invoke(logicScene, sceneData);
                }
            }
        }
        
        private void SceneManagerLoaded(Scene scene, LoadSceneMode mode)
        {
            OnSceneLoaded?.Invoke(scene.name);
        }

        public void SetActiveScene(string environmentName)
        {
            var scene = SceneManager.GetSceneByName(environmentName);
            SceneManager.SetActiveScene(scene);
        }

        public void ShowCredits()
        {
            CameraFade.FadeIn(0.5f, () => LoadScene(creditsScene, fadeCamera: false), FadeMethod.PostProcess);
        }
    }
}