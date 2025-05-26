using System.Collections;
using DreadZitoEngine.Runtime.Gameplay.InteractionSystem;
using DreadZitoEngine.Runtime.Inventory;
using DreadZitoEngine.Runtime.Memos;
using DreadZitoEngine.Runtime.UI.System;
using UnityEngine;

namespace DreadZitoEngine.Runtime.UI.Gameplay
{
    [RequireComponent(typeof(HotspotUI))]
    public class PlayerMenuController : MenuController
    {
        // UI Controllers
        private HotspotUI hotspotUI;

        [SerializeField] private Canvas canvas;

        [Header("Settings")]
        [SerializeField] private int maxInventoryNotifications = 4;
        private int currentInventoryNotifications = 0;
        
        [Header("Panels")]
        [SerializeField] private Panel hotspotNamePanel;
        [SerializeField] private Panel questNoteNotification;

        private Coroutine questNoteNotificationRoutine;
        
        public void Init()
        {
            if (!hotspotUI)
                hotspotUI = GetComponent<HotspotUI>();
            if (!canvas)
                canvas = GetComponent<Canvas>();
            
            hotspotUI.Init(canvas, hotspotNamePanel);
        }

        private void Update()
        {
            //var cancelInput = GameplayMain.Instance.Player.Input.UI.Cancel;
            if (/*cancelInput.WasPressedThisFrame()*/ Input.GetKeyDown(KeyCode.Escape))
            {
                PopPanel();
            }
        }

        #region Show / Hide methods
        
        public void ShowHotspotName(Hotspot hotspot)
        {
            hotspotUI.ShowHotspotName(hotspot);
            PushPanel(hotspotNamePanel);
        }
        
        public void HideHotspotName()
        {
            PopPanel(hotspotNamePanel);
        }
        
        #endregion

        public void ShowQuestNoteNotification()
        {
            if (questNoteNotification.IsVisible)
                return;

            questNoteNotificationRoutine = StartCoroutine(ShowQuestNoteNotificationRoutine());
        }
        
        public IEnumerator ShowQuestNoteNotificationRoutine()
        {
            PushPanel(questNoteNotification);
            yield return new WaitForSeconds(10f);
            PopPanel(questNoteNotification, true);
        }

        public void TryHideQuestNoteNotification()
        {
            if (questNoteNotificationRoutine != null)
            {
                StopCoroutine(questNoteNotificationRoutine);
                questNoteNotificationRoutine = null;
            }
            
            PopPanel(questNoteNotification, true);
        }
    }
}