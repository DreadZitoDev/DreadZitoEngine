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
        
        public const string CUTSCENE_MOVE_BLOCKER_ID = "cutscene";
        public const string INTERACTION_MOVE_BLOCKER_ID = "interaction";
        
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
        
        public virtual void PlayerLockMovement(string interactionMoveBlockerID, bool value)
        {
            if (interactionMoveBlockerID == CUTSCENE_MOVE_BLOCKER_ID)
            {
                Player.AddMoveBlocker(interactionMoveBlockerID, 10, value);
            }
            else if (interactionMoveBlockerID == INTERACTION_MOVE_BLOCKER_ID)
            {
                Player.AddMoveBlocker(interactionMoveBlockerID, 9, value);
            }
            else
                return;
        }

        public static T GetInstance<T>() where T : GameplayMain
        {
            if (Instance is T gameplayMain)
            {
                return gameplayMain;
            }
            Debug.LogError($"GameplayMain instance is not of type {typeof(T).Name}");
            return null;
        }
    }
}