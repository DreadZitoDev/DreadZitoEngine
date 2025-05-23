using UnityEngine.InputSystem;

namespace _Room502.Scripts
{
    public static class InputBridge
    {
        private static GameInput input;

        public static GameInput Inputs
        {
            get
            {
                if (input != null) return input;
                input = new GameInput();
                return input;
            }
        }

        public static InputAction Move => Inputs.Move;
        public static InputAction Interact => Inputs.Interact;
        public static InputAction ToggleQuestNote => Inputs.ToggleQuestNote;
        public static InputAction ToggleInventory => Inputs.ToggleInventory;
        public static InputAction ExamineInteractionLeave => Inputs.ExamineInteractionLeave;
    }
}
