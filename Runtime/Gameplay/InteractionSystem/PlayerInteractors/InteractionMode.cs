namespace DreadZitoEngine.Runtime.Gameplay.InteractionSystem
{
    public interface InteractionMode
    {
        public Hotspot DetectHotspot();
        public void ExitMode();
    }
}