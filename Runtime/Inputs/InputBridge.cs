using UnityEngine.InputSystem;

namespace DreadZitoEngine.Runtime.Inputs
{
    public static class InputBridge
    {
        private static GameInput Input;

        public static void Init(GameInput input)
        {
            Input = input;
        }

        public static InputAction Move => Input.Move();
        public static InputAction Interact => Input.Interact();
        public static InputAction ToggleQuestNote => Input.ToggleQuestLog();
        public static InputAction ToggleInventory => Input.ToggleInventory();
        public static InputAction ExamineInteractionLeave => Input.ExamineInteractionLeave();
        public static InputAction Cancel => Input.ExamineInteractionLeave(); // Assuming Cancel is the same as ExamineInteractionLeave
    }
}
