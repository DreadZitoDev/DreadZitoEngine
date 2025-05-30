﻿// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Add this script to your Dialogue Manager to keep track of the 
    /// current conversation and dialogue entry. When you load a game,
    /// it will resume the conversation at that point.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class ConversationStateSaver : Saver
    {
        [Serializable]
        public class Data
        {
            public int conversationID;
            public int entryID;
            public string actorName;
            public string conversantName;
            public List<string> actorGOs = null;
            public List<SubtitlePanelNumber> actorGOPanels = null;
            public List<int> actorIDs = null;
            public List<SubtitlePanelNumber> actorIDPanels = null;
            public List<string> panelOpenOnActorName = null;
            public string accumulatedText;
        }

        /// <summary>
        /// Override to make default key value "ConversationState".
        /// </summary>
        public override string key
        {
            get
            {
                if (string.IsNullOrEmpty(m_runtimeKey))
                {
                    m_runtimeKey = !string.IsNullOrEmpty(_internalKeyValue) ? _internalKeyValue : "ConversationState";
                    if (appendSaverTypeToKey)
                    {
                        var typeName = GetType().Name;
                        if (typeName.EndsWith("Saver")) typeName.Remove(typeName.Length - "Saver".Length);
                        m_runtimeKey += typeName;
                    }
                }
                return m_runtimeKey;
            }
            set
            {
                _internalKeyValue = value;
                m_runtimeKey = value;
            }
        }

        public override string RecordData()
        {
            if (!DialogueManager.isConversationActive) return string.Empty;
            var data = new Data();
            var state = DialogueManager.currentConversationState;
            var entry = state.subtitle.dialogueEntry;
            data.conversationID = entry.conversationID;
            data.entryID = state.subtitle.dialogueEntry.id;
            data.actorName = (DialogueManager.currentActor != null) ? DialogueManager.currentActor.name : string.Empty;
            data.conversantName = (DialogueManager.currentConversant != null) ? DialogueManager.currentConversant.name : string.Empty;
            var ui = DialogueManager.dialogueUI as StandardDialogueUI;
            if (ui != null)
            {
                ui.conversationUIElements.standardSubtitleControls.RecordActorPanelCache(out data.actorGOs, out data.actorGOPanels, out data.actorIDs, out data.actorIDPanels, out data.panelOpenOnActorName);
                data.accumulatedText = string.Empty;
                for (int i = 0; i < ui.conversationUIElements.subtitlePanels.Length; i++)
                {
                    var subtitlePanel = ui.conversationUIElements.subtitlePanels[i];
                    if (!subtitlePanel.isOpen && 0 <= i && i < data.panelOpenOnActorName.Count)
                    {
                        data.panelOpenOnActorName[i] = null;
                    }
                    if (subtitlePanel.isOpen && subtitlePanel.accumulateText)
                    {
                        data.accumulatedText = subtitlePanel.accumulatedText;
                        break;
                    }
                }
            }
            return SaveSystem.Serialize(data);
        }

        public override void ApplyData(string s)
        {
            if (!enabled || string.IsNullOrEmpty(s)) return;
            var data = SaveSystem.Deserialize<Data>(s);
            if (data == null) return;
            var dialogueUI = DialogueManager.dialogueUI as StandardDialogueUI;
            if (dialogueUI != null) dialogueUI.CloseImmediately();
            DialogueManager.StopConversation();
            var conversationID = data.conversationID;
            var entryID = data.entryID;
            var conversation = DialogueManager.masterDatabase.GetConversation(conversationID);
            var actorName = DialogueLua.GetVariable("CurrentConversationActor").AsString;
            var conversantName = DialogueLua.GetVariable("CurrentConversationConversant").AsString;
            if (DialogueDebug.logInfo) Debug.Log("Dialogue System: ConversationStateSaver is resuming conversation " + conversation.Title + " with actor=" + actorName + " and conversant=" + conversantName + " at entry " + entryID + ".", this);
            var actor = string.IsNullOrEmpty(actorName) ? null : GameObject.Find(actorName);
            var conversant = string.IsNullOrEmpty(conversantName) ? null : GameObject.Find(conversantName);
            var actorTransform = (actor != null) ? actor.transform : null;
            var conversantTransform = (conversant != null) ? conversant.transform : null;
            var ui = DialogueManager.dialogueUI as StandardDialogueUI;
            if (ui != null)
            {
                ui.conversationUIElements.standardSubtitleControls.QueueSavedActorPanelCache(data.actorGOs, data.actorGOPanels, data.actorIDs, data.actorIDPanels);
            }
            DialogueManager.StartConversation(conversation.Title, actorTransform, conversantTransform, entryID);
            if (ui != null)
            {
                for (int i = 0; i < ui.conversationUIElements.subtitlePanels.Length; i++)
                {
                    var subtitlePanel = ui.conversationUIElements.subtitlePanels[i];
                    if (0 <= i && i < data.panelOpenOnActorName.Count && !string.IsNullOrEmpty(data.panelOpenOnActorName[i]))
                    {
                        var panelActorTransform = CharacterInfo.GetRegisteredActorTransform(data.panelOpenOnActorName[i]);
                        var dialogueActor = (panelActorTransform != null) ? panelActorTransform.GetComponent<DialogueActor>() : null;
                        var panelActor = DialogueManager.masterDatabase.GetActor(data.panelOpenOnActorName[i]);
                        Sprite portraitSprite = (panelActor != null) ? panelActor.GetPortraitSprite() : null;
                        string portraitName = data.panelOpenOnActorName[i];
                        if (dialogueActor != null)
                        {
                            var dialogueActorSprite = dialogueActor.GetPortraitSprite();
                            if (dialogueActorSprite != null) portraitSprite = dialogueActorSprite;
                            portraitName = dialogueActor.GetActorName();
                        }
                        else if (panelActor != null)
                        {
                            portraitSprite = panelActor.GetPortraitSprite();
                            portraitName = CharacterInfo.GetLocalizedDisplayNameInDatabase(portraitName);
                        }
                        if (!subtitlePanel.isOpen)
                        {
                            subtitlePanel.OpenOnStartConversation(portraitSprite, portraitName, dialogueActor);
                        }
                    }
                    if (subtitlePanel.accumulateText)
                    {
                        subtitlePanel.accumulatedText = data.accumulatedText;
                    }
                }

                // Restore continue button state:
                // Note: Doesn't account for SetContinueMode() since we haven't been saving that info
                // in RecordData() and we don't want to break older saves.
                if (ShouldShouldContinueButton(DialogueManager.currentConversationState))
                {
                    DialogueActor currentDialogueActor;
                    var currentPanel = ui.conversationUIElements.standardSubtitleControls.GetPanel(DialogueManager.currentConversationState.subtitle, out currentDialogueActor);
                    currentPanel.ShowContinueButton();
                }
            }
        }

        private bool ShouldShouldContinueButton(ConversationState state)
        {
            switch (DialogueManager.displaySettings.subtitleSettings.continueButton)
            {
                default:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.Always:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.Optional:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OptionalBeforeResponseMenu:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OptionalBeforePCAutoresponseOrMenu:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OptionalForPC:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OptionalForPCOrBeforeResponseMenu:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OptionalForPCOrBeforePCAutoresponseOrMenu:
                    return true;
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.Never:
                    return false;
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.NotBeforeResponseMenu:
                    return state.hasPCResponses && !state.hasPCAutoResponse;
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.NotBeforePCAutoresponseOrMenu:
                    return state.hasPCResponses;
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.NotForPC:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.NotForPCOrBeforeResponseMenu:
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.NotForPCOrBeforePCAutoresponseOrMenu:
                    return !state.hasPCResponses;
                case DisplaySettings.SubtitleSettings.ContinueButtonMode.OnlyForPC:
                    return !state.HasNPCResponse;
            }
        }

    }
}
