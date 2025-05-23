using System;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.UI.System.PageAnim
{
    public abstract class PanelAnimation : MonoBehaviour
    {
        public UnityEvent OnEnterAnimationEnd;
        public UnityEvent OnExitAnimationEnd;
        
        public abstract void Enter(Panel panel, Action onEnd = null);
        public abstract void Exit(Panel panel, Action onEnd = null);
    }
}