using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using VSNL.UI;
using VSNLEngine.Core;

namespace VSNL.Core
{
    public class UIManager : MonoBehaviour, IGameService
    {
        [Header("Dialogue UI")]
        public GameObject DialoguePanel;
        public TextMeshProUGUI SpeakerText;
        public TextMeshProUGUI DialogueText;
        public float TypewriterSpeed = 0.05f;

        [Header("Backlog")]
        // public List<string> Backlog = new List<string>(); // Use BacklogManager instead
        // public int MaxBacklogSize = 50;

        [Header("Choice UI")]
        public GameObject ChoicePanel;
        public GameObject ChoiceButtonPrefab;
        public Transform ChoiceContainer;

        private bool _isTyping;
        private string _fullText;
        private ScriptPlayer _scriptPlayer;
        private List<GameObject> _activeChoiceButtons = new List<GameObject>();

        [Header("Text System")]
        public TextDisplayManager TextManager;

        public async UniTask InitializeAsync()
        {
            Debug.Log("[UIManager] Initialized.");
            _scriptPlayer = Engine.Instance.GetService<ScriptPlayer>();
            
            // Auto-link if on same object
            if (!TextManager) TextManager = GetComponent<TextDisplayManager>();
            if (!TextManager && DialoguePanel) TextManager = DialoguePanel.GetComponent<TextDisplayManager>();
            
            // Link Internal components to Manager if Manager is missing refs (Optional convenience)
            if (TextManager)
            {
                if (!TextManager.SpeakerText) TextManager.SpeakerText = SpeakerText;
                if (!TextManager.DialogueText) TextManager.DialogueText = DialogueText;
            }

            if (DialoguePanel) DialoguePanel.SetActive(false);
            if (ChoicePanel) ChoicePanel.SetActive(false);
            
            // Subscribe to Input
            var input = Engine.Instance.GetService<InputService>();
            if (input != null)
            {
                input.OnHideToggle += ToggleDialogueVisibility;
            }

            await UniTask.CompletedTask;
        }

        public void SetDialoguePanelVisibility(bool visible)
        {
            if (DialoguePanel) DialoguePanel.SetActive(visible);
        }

        private void ToggleDialogueVisibility()
        {
            if (DialoguePanel)
            {
                DialoguePanel.SetActive(!DialoguePanel.activeSelf);
            }
        }

        public void ResetService()
        {
            HideDialogue();
            HideChoices();
        }

        #region Dialogue System
        public async UniTask ShowDialogueAsync(string speaker, string text)
        {
            if (DialoguePanel) DialoguePanel.SetActive(true);
            AddToBacklog(speaker, text);

            if (TextManager)
            {
                // Delegate to Manager with state checks
                await TextManager.ShowTextAsync(
                    speaker, 
                    text, 
                    () => _scriptPlayer != null && _scriptPlayer.IsAutoPlay,
                    () => _scriptPlayer != null && _scriptPlayer.IsSkipping
                );
            }
            else
            {
                // Fallback (or Error)
                Debug.LogWarning("[UIManager] TextDisplayManager missing!");
                if (SpeakerText) SpeakerText.text = speaker;
                if (DialogueText) DialogueText.text = text;
                // Simple wait
                while (!Input.GetMouseButtonDown(0)) await UniTask.Yield();
            }
        }

        public void HideDialogue()
        {
            if (DialoguePanel) DialoguePanel.SetActive(false);
        }
        #endregion

        #region Choice System
        /// <summary>
        /// Adds a choice option to the pending list.
        /// </summary>
        public void AddChoice(string text, string targetLabel)
        {
            if (!ChoicePanel) return;
            // We instantiate immediately but panel might be hidden until @stop
            // Ideally we store data, but instantiating into a hidden container is easier for this phase.
            
            if (!ChoiceButtonPrefab || !ChoiceContainer)
            {
                Debug.LogError("[UIManager] Choice Prefab or Container missing!");
                return;
            }

            GameObject btnObj = Instantiate(ChoiceButtonPrefab, ChoiceContainer);
            ChoiceButton btn = btnObj.GetComponent<ChoiceButton>();
            if (btn)
            {
                btn.Setup(text, targetLabel, OnChoiceSelected);
            }
            _activeChoiceButtons.Add(btnObj);
        }

        public void ShowChoices()
        {
            if (ChoicePanel) ChoicePanel.SetActive(true);
            if (DialoguePanel) DialoguePanel.SetActive(false); // Hide dialogue during choice? Optional style.
        }

        public void HideChoices()
        {
            if (ChoicePanel) ChoicePanel.SetActive(false);
            foreach (var btn in _activeChoiceButtons)
            {
                Destroy(btn);
            }
            _activeChoiceButtons.Clear();
        }

        private void OnChoiceSelected(string targetLabel)
        {
            // Resume Script
            HideChoices();
            if (DialoguePanel) DialoguePanel.SetActive(true); // Restore dialogue box if needed

            if (_scriptPlayer != null)
            {
                // If label is valid, jump. If null/empty, just continue.
                if (!string.IsNullOrEmpty(targetLabel))
                {
                    _scriptPlayer.JumpToLabel(targetLabel);
                }
                _scriptPlayer.ResumeFromChoice();
            }
        }

        private void AddToBacklog(string speaker, string text)
        {
            var backlog = Engine.Instance.GetService<BacklogManager>();
            if (backlog != null)
            {
                backlog.AddLog(speaker, text);
            }
        }
        
        // Optional: Call this from a UI Button
        public void ToggleBacklog()
        {
            var backlogUI = FindFirstObjectByType<BacklogUI>(); // Or cache it
            if (backlogUI) backlogUI.Toggle();
        }
        #endregion
    }
}

