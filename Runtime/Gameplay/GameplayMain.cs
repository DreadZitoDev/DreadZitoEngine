using System.Linq;
using DreadZitoEngine.Runtime.Feedbacks;
using DreadZitoEngine.Runtime.Gameplay.InteractionSystem;
using DreadZitoEngine.Runtime.Gameplay.Players;
using DreadZitoEngine.Runtime.Scenes;
using DreadZitoEngine.Runtime.UI.Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreadZitoEngine.Runtime.Gameplay
{
    public abstract class GameplayMain : MonoBehaviour
    {
        public static GameplayMain Instance { get; private set; }

        [SerializeField] private GameSceneData gameplaySceneData;
        [SerializeField] private Player playerPrefab;
        
        private Player player;
        public Player Player => player;
        
        [SerializeField] private PlayerMenuController menuController;
        public PlayerMenuController MenuController => menuController;
        [SerializeField] private SFX sfx;
        
        public InteractionSystemHandler InteractionSystemHandler { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            var playerSpawn = FindObjectOfType<Tags.PlayerSpawn>();
            
            player ??= FindObjectOfType<Player>();
            player ??= Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
            player.Init();
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(gameplaySceneData.LogicSceneNames.First()));
            
            menuController ??= FindObjectOfType<PlayerMenuController>();
            
            menuController.Init();
            
            InteractionSystemHandler ??= GetComponent<InteractionSystemHandler>();
            InteractionSystemHandler.Init(player, menuController);

            menuController.ShowQuestNoteNotification();
            
            sfx.Init();
        }


        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            StopAllCoroutines();
        }
    }
}