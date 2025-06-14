using System;
using DreadZitoEngine.Runtime.Gameplay.InteractionSystem;
using DreadZitoEngine.Runtime.Inputs;
using DreadZitoEngine.Runtime.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreadZitoEngine.Runtime.Gameplay.Players
{
    public abstract class Player : MonoBehaviour
    {
        [field:SerializeField] public Transform Camera;
        public PlayerInteractor Interactor { get; private set; }
        public Animator Animator;

        public InventorySystem Inventory;

        public GameObject Model;
        
        public event Action OnInteractInput;
        public event Action<bool> OnQuestNoteToggle;
        
        public Hotspot CurrentHotspot => Interactor.CurrentHotspot;

        private PlayerThirdPersonAnimator playerThirdPersonAnimator;
        
        private PlayerMoveBlocker playerMoveBlocker = new();

        public void Init()
        {
            Interactor = GetComponent<PlayerInteractor>();
            playerThirdPersonAnimator ??= GetComponentInChildren<PlayerThirdPersonAnimator>();
            Inventory ??= GetComponentInChildren<InventorySystem>();
            
            Interactor?.Init();
        
            InputBridge.Interact.performed += InteractInput;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (!Model)
                Model = GetComponentInChildren<SkinnedMeshRenderer>().gameObject;
            
            playerThirdPersonAnimator?.Init();
        }

        private void Update()
        {
            bool shouldBlock = playerMoveBlocker.ShouldBlockMovement();
            SetCanMove(!shouldBlock);
            
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            // ALLOW TO TOGGLE CURSOR FOR DEBUGGING
            if (Input.GetKey(KeyCode.F2))
            {
                SetCursorVisibility(!Cursor.visible);
            }
            #endif
        }

        private void SetCursorVisibility(bool value)
        {
            Cursor.lockState = value ? CursorLockMode.Confined : CursorLockMode.Locked;
            Cursor.visible = value;
        }
        

        private void InteractInput(InputAction.CallbackContext obj)
        {
            if (obj.performed)
                OnInteractInput?.Invoke();
        }

        public void TeleportTo(Vector3 position)
        {
            transform.position = position;
        }
        
        public void TeleportTo(Transform target, bool copyRotation = true)
        {
            if (copyRotation)
            {
                transform.position = target.position;
                transform.rotation = target.rotation;
            }
            else
            {
                transform.position = target.position;
            }
        }

        public void SetModelVisibility(bool value)
        {
            Model.SetActive(value);
        }
        
        public void AddMoveBlocker(string sourceId, int priority, bool lockValue)
        {
            playerMoveBlocker.AddBlocker(sourceId, priority, lockValue);
        }
        
        public void RemoveMoveBlocker(string sourceId)
        {
            playerMoveBlocker.RemoveBlocker(sourceId);
        }

        public void AddItem(ItemDataSO itemData)
        {
            Inventory.AddItem(itemData);
        }

        private void SetCanMove(bool value)
        {
            //Controller.playerCanMove = value;
            //playerThirdPersonAnimator.enabled = value;
            //throw new NotImplementedException();
        }

        public void SetCanMoveCamera(bool value)
        {
            //Controller.cameraCanMove = value;
            //throw new NotImplementedException();
        }
        
        public void SetCanInteract(bool value)
        {
            Interactor.SetCanInteract(value);
        }
        
        public Vector3 GetPosition() => transform.position;
        public bool IsMoving => /*Controller.IsMoving*/ false;

        public string PrintMoveBlockers() => playerMoveBlocker.ToString();

        public void SetGravity(bool value)
        {
            //Controller.SetGravity(value);
            throw new NotImplementedException();
        }

        private void OnDestroy()
        {
            InputBridge.Interact.performed -= InteractInput;
        }
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Player))]
    public class PlayerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            var player = (Player) target;
            if (GUILayout.Button("Show Move Blockers"))
            {
                Debug.Log($"{player.PrintMoveBlockers()}");
            }
        }
    }
    #endif
}