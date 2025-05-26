using UnityEngine.InputSystem;

namespace DreadZitoEngine.Runtime.Inputs
{
    public abstract class GameInput
    {
        public abstract InputAction Move();
        public abstract InputAction Interact();
        public abstract InputAction ToggleQuestLog();
        public abstract InputAction ToggleInventory();
        public abstract InputAction ExamineInteractionLeave();
    }
}