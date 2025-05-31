using System.Linq;
using DreadZitoEngine.Runtime.Feedbacks;
using DreadZitoEngine.Runtime.Gameplay.InteractionSystem;
using DreadZitoEngine.Runtime.Gameplay.Players;
using DreadZitoEngine.Runtime.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreadZitoEngine.Runtime.Gameplay
{
    [DefaultExecutionOrder(-1)]
    public class GameplayMain : MonoBehaviour
    {
        public static GameplayMain Instance { get; private set; }

        [SerializeField] private GameSceneData gameplaySceneData;
        [SerializeField] private Player playerPrefab;
        
        private Player player;
        public Player Player => player;

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

        protected virtual void Initialize()
        {
            var playerSpawn = FindObjectOfType<Tags.PlayerSpawn>();
            
            player ??= FindObjectOfType<Player>();
            player ??= Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
            player.Init();
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(gameplaySceneData.LogicSceneNames.First()));

            InteractionSystemHandler ??= GetComponent<InteractionSystemHandler>();
            InteractionSystemHandler?.Init(player);

            sfx?.Init();
        }


        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            StopAllCoroutines();
        }
    }
}