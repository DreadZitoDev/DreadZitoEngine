using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DreadZitoEngine.Runtime.UI.System
{
    public class MenuController : MonoBehaviour
    {
        internal Stack<Panel> panelStack = new Stack<Panel>();
 
        public UnityEvent<Panel> OnPanelPushed;
        public UnityEvent<Panel> OnPanelPopped;

        public void PushPanel(Panel panel)
        {
            if (PanelIsOnTop(panel)) {
                //panel.SetFocus();
                return;
            }

            Debug.Log($"Pushing panel {panel.name} - stack count: {panelStack.Count}");
            
            
            if (panelStack.Count > 0) {
                var oldPanel = panelStack.Peek();
                if (oldPanel.ExitOnNewPanelEnter)
                    oldPanel.Exit();
                // The new panel has any selectable? then override focus
                else if (panel.FirstSelected != null)
                    oldPanel.SetUnFocus();
            }

            panelStack.Push(panel);
            panel.Enter();
            
            OnPanelPushed?.Invoke(panel);
        }
        
        public bool PopPanel(bool force = false)
        {
            if (panelStack.Count == 0)
                return false;
            var topPanel = GetTopPanel();
            if (topPanel != null && (!force && !topPanel.IsSkippable))
            {
                Debug.LogWarning($"PopPanel Panel {topPanel.name} is not skippable");
                return false;
            }

            var curPanel = panelStack.Pop();
            curPanel.Exit();
            OnPanelPopped?.Invoke(curPanel);

            if (panelStack.Count > 0)
            {
                var backPanel = panelStack.Peek();
                Debug.Log($"Recovering focus on panel {backPanel.name}");
                if (backPanel.ExitOnNewPanelEnter)
                    backPanel.Enter();
                else
                    backPanel.SetFocus();
            }

            return true;
        }
        
        public void PopAllPanels()
        {
            while (panelStack.Count > 0)
            {
                var popPanel = PopPanel();
                if (!popPanel)
                    break;
            }
        }
        
        public void PopPanelUntil(Panel panel, bool inclusive = false)
        {
            while (panelStack.Count > 0 && panelStack.Peek() != panel)
                PopPanel();
            // We are at the panel we want to stop at
            if (inclusive)
                PopPanel();
        }
        
        private void BringPanelToTop(Panel panel)
        {
            if (panelStack.Count == 0)
                return;
            if (panelStack.Peek() == panel)
                return;
            if (panelStack.Contains(panel))
            {
                var panels = panelStack.ToList();
                panels.Remove(panel);
                panels.Add(panel);
                panelStack = new Stack<Panel>(panels);
            }
        }
        
        public void PopPanel(Panel panel, bool force = false)
        {
            if (panelStack.Count == 0)
                return;
            if (force)
            {
                // Bring panel to first place and then pop it
                BringPanelToTop(panel);
                PopPanel(true);
                return;
            }
            if (PanelIsOnTop(panel))
                PopPanel();
        }
        
        public bool PanelIsOnTop(Panel panel)
        {
            return panelStack.Count > 0 && panelStack.Peek() == panel;
        }
        
        public Panel GetTopPanel()
        {
            return panelStack.Count > 0 ? panelStack.Peek() : null;
        }
        
        public int ActivePanelsCount => panelStack.Count;
        
        public void SetAllPanelsInteractable(bool value)
            => panelStack.ToList().ForEach(e => e.SetInteractable(value));
    }
}