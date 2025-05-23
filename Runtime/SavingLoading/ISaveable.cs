using System;

namespace DreadZitoEngine.Runtime.SavingLoading
{
    public interface ISaveable
    {
        public object CaptureState();
        public void RestoreState(object state, Action onLoadComplete = null);
    }
}