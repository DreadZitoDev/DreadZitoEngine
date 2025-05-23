using DreadZitoEngine.Runtime.UI.System;
using UnityEngine;
using UnityEngine.UI;

namespace DreadZitoEngine.Runtime
{
    public class MainMenu : MenuController
    {
        [SerializeField] private Panel mainPanel;
        [SerializeField] private Panel loadGamePanel;
        [SerializeField] private Panel settingsPanel;

        [Header("Main panel")]
        [SerializeField] private Selectable continueGameButton;
        [SerializeField] private Selectable newGameButton;
        
        private void Start()
        {
            PushPanel(mainPanel);
            // Here check if there is a save file, if there is, enable continue button
            
            // If there is no save file, disable continue button
            if (!continueGameButton.gameObject.activeSelf)
                mainPanel.SetFirstSelected(newGameButton);
        }

        public void NewGame()
        {
            SetAllPanelsInteractable(false);
            Game.Instance.NewGame();
        }
        
        public void LoadGame()
        {
            SetAllPanelsInteractable(false);
        }
        
        public void Continue()
        {
            SetAllPanelsInteractable(false);
        }
        
        public void Options()
        {
            SetAllPanelsInteractable(false);
        }
        
        public void Exit()
        {
            SetAllPanelsInteractable(false);
            Application.Quit();
        }
    }
}