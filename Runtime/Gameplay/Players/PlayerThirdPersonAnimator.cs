using DreadZitoEngine.Runtime.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreadZitoEngine.Runtime.Gameplay.Players
{
    public class PlayerThirdPersonAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject playerPivot;
        [SerializeField] private Animator thirdPersonAnimator;

        private Vector2 moveInput;
        private Vector3 moveVelocityDamp;
        
        public void Init()
        {
           InputBridge.Move.performed += Move;
           InputBridge.Move.canceled += Move;
        }

        private void Move(InputAction.CallbackContext obj)
        {
            moveInput = obj.ReadValue<Vector2>();
        }
        
        private void Update()
        {
            var moveInputToLocal = playerPivot.transform.InverseTransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
            var current = new Vector3(thirdPersonAnimator.GetFloat("Horizontal"), 0, thirdPersonAnimator.GetFloat("Vertical"));
            var target = Vector3.SmoothDamp(current, moveInputToLocal, ref moveVelocityDamp, 0.1f);
            
            thirdPersonAnimator.SetFloat("Horizontal", target.x);
            thirdPersonAnimator.SetFloat("Vertical", target.z);
        }

        // TODO: Workaround, NEED URGENTLY A INPUT PROVIDER FOR THIS AND PLAYER CONTROLLER SCRIPT TO SHARE INPUTS
        private void OnDisable()
        {
            thirdPersonAnimator.SetFloat("Horizontal", 0);
            thirdPersonAnimator.SetFloat("Vertical", 0);
        }

        private void OnDestroy()
        {
            InputBridge.Move.performed -= Move;
            InputBridge.Move.canceled -= Move;
        }
    }
}