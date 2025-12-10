using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VSNL.State;
using VSNL.Core; // Keep for existing interfaces/types if needed
using VSNLEngine.Core.Data;
using VSNLEngine.Core.Parser;

namespace VSNLEngine.Core
{
    /// <summary>
    /// Handles the execution of VSNL scripts.
    /// Manages control flow, commands, and dialogue presentation.
    /// </summary>
    public class ScriptPlayer : MonoBehaviour, IGameService
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the script is currently running.
        /// </summary>
        public bool IsPlaying { get; private set; }

        /// <summary>
        /// Gets a value indicating whether functionality is paused waiting for user choice.
        /// </summary>
        public bool IsWaitingForChoice { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is in Auto-Play mode.
        /// </summary>
        public bool IsAutoPlay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player is skipping text.
        /// </summary>
        public bool IsSkipping { get; set; }

        /// <summary>
        /// Gets or sets the current line index in the script.
        /// </summary>
        public int CurrentLineIndex { get; set; } = 0;

        /// <summary>
        /// Gets the name of the currently loaded script file.
        /// </summary>
        public string CurrentScriptFile { get; private set; }

        #endregion

        #region Private Fields

        private ScriptData _currentScriptData;
        private SaveLoadManager _stateManager;
        private System.Threading.CancellationTokenSource _playCts;

        #endregion

        #region IGameService Implementation

        /// <summary>
        /// Initializes the service asynchronously.
        /// </summary>
        public async UniTask InitializeAsync()
        {
            _stateManager = Engine.Instance.GetService<SaveLoadManager>();
            Debug.Log("[ScriptPlayer] Initialized.");
            
            var loc = Engine.Instance.GetService<LocalizationManager>();
            if (loc != null) loc.OnLanguageChanged += OnLanguageChanged;

            await UniTask.CompletedTask;
        }

        /// <summary>
        /// Resets the service state.
        /// </summary>
        public void ResetService()
        {
            _currentScriptData = null;
            CurrentLineIndex = 0;
            IsPlaying = false;
        }

        #endregion

        #region Private Methods

        private void OnLanguageChanged()
        {
            // Reload current script at current line
            if (!string.IsNullOrEmpty(CurrentScriptFile))
            {
                 int savedIndex = CurrentLineIndex;
                 LoadScriptAsync(CurrentScriptFile).Forget(); 
                 // Restore index
                 CurrentLineIndex = savedIndex; // Simple restore
            }
        }
        
        // Helper for string path (from Localization or Restore)
        private async UniTask LoadScriptAsync(string pathOrName)
        {
             var resources = Engine.Instance.GetService<ResourceManager>();
             if (resources != null)
             {
                 // Assuming path is just name for now or full path
                 string loadPath = pathOrName.Contains("/") ? pathOrName : $"Scripts/{pathOrName}";
                 var asset = await resources.LoadAssetAsync<TextAsset>(loadPath);
                 if (asset) LoadScript(asset);
             }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads and parses a script file.
        /// </summary>
        /// <param name="scriptFile">The text asset to load.</param>
        public void LoadScript(TextAsset scriptFile)
        {
            ResetService();
            _stateManager.CurrentState.ScriptFileName = scriptFile.name;
            CurrentScriptFile = scriptFile.name;

            // Delegate Parsing to ScriptParser (SRP)
            _currentScriptData = ScriptParser.Parse(scriptFile);
            
            Debug.Log($"[ScriptPlayer] Loaded script {_currentScriptData.ScriptName} with {_currentScriptData.Lines.Count} lines.");
        }

        /// <summary>
        /// Starts executing the loaded script asynchronously.
        /// </summary>
        public async UniTask PlayScriptAsync()
        {
            if (_currentScriptData == null || _currentScriptData.Lines.Count == 0)
            {
                Debug.LogWarning("[ScriptPlayer] No script loaded.");
                return;
            }

            IsPlaying = true;
            _playCts?.Cancel();
            _playCts = new System.Threading.CancellationTokenSource();

            int loopSafetyCounter = 0;
            
            try 
            {
                while (IsPlaying && CurrentLineIndex < _currentScriptData.Lines.Count)
                {
                    if (_playCts.Token.IsCancellationRequested) break;

                    if (IsWaitingForChoice)
                    {
                        loopSafetyCounter = 0; // Reset on wait
                        await UniTask.Yield(); 
                        continue;
                    }

                    // Infinite Loop Guard
                    loopSafetyCounter++;
                    if (loopSafetyCounter > 1000)
                    {
                        Debug.LogError("[ScriptPlayer] Infinite Loop detected! Force waiting.");
                        loopSafetyCounter = 0;
                        await UniTask.Yield();
                    }

                    var line = _currentScriptData.Lines[CurrentLineIndex];
                    _stateManager.CurrentState.LineIndex = CurrentLineIndex;
                    
                    try
                    {
                        Debug.Log($"[ScriptPlayer] Executing Line {CurrentLineIndex}: {line.RawContent}");
                        await ExecuteLineAsync(line);
                        // If line execution took time (await), reset safety counter
                        if (line.Type == VSNLLineType.Dialogue || line.CommandName == "wait") 
                        {
                            loopSafetyCounter = 0;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[ScriptPlayer] Error at line {CurrentLineIndex}: {ex.Message}");
                    }
                    
                    // Check again in case a Jump happened or we paused
                    if (!IsWaitingForChoice && !(_playCts.Token.IsCancellationRequested))
                    {
                        CurrentLineIndex++;
                    }
                }
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("[ScriptPlayer] Execution Cancelled.");
            }

            IsPlaying = false;
            Debug.Log("[ScriptPlayer] Script Finished.");
        }

        /// <summary>
        /// Pauses execution to wait for a player choice.
        /// </summary>
        public void PauseForChoice()
        {
            IsWaitingForChoice = true;
        }

        /// <summary>
        /// Resumes execution after a choice has been made.
        /// </summary>
        public void ResumeFromChoice()
        {
            IsWaitingForChoice = false;
        }

        /// <summary>
        /// Jumps the execution pointer to the specified label.
        /// </summary>
        /// <param name="label">The target label string.</param>
        public void JumpToLabel(string label)
        {
            if (_currentScriptData != null && _currentScriptData.LabelMap.ContainsKey(label))
            {
                // Set index to the label line. 
                CurrentLineIndex = _currentScriptData.LabelMap[label]; 
            }
            else
            {
                Debug.LogError($"[ScriptPlayer] Label not found: {label}");
            }
        }

        /// <summary>
        /// Jumps to a specific line index.
        /// </summary>
        /// <param name="index">The 0-based line index.</param>
        public void JumpToLine(int index)
        {
            if (_currentScriptData != null && index >= 0 && index < _currentScriptData.Lines.Count)
            {
                CurrentLineIndex = index;
            }
        }
        
        /// <summary>
        /// Skips logic lines until a target command is found (e.g. for If/Else blocks).
        /// </summary>
        /// <param name="targetCommandNames">Array of command names to stop at.</param>
        public void SkipToNextCommand(string[] targetCommandNames)
        {
            if (_currentScriptData == null) return;

            int depth = 0; 
            
            while (CurrentLineIndex < _currentScriptData.Lines.Count - 1)
            {
                CurrentLineIndex++;
                var line = _currentScriptData.Lines[CurrentLineIndex];
                
                if (line.Type == VSNLLineType.Command)
                {
                    // Handle nesting
                    if (line.CommandName == "if") depth++;
                    if (line.CommandName == "endif")
                    {
                         if (depth > 0) 
                         {
                             depth--; 
                             continue;
                         }
                    }

                    // Found target?
                    if (depth == 0)
                    {
                        foreach (var target in targetCommandNames)
                        {
                            if (line.CommandName == target) return; 
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Execution

        private async UniTask ExecuteLineAsync(VSNLLine line)
        {
            switch (line.Type)
            {
                case VSNLLineType.Comment:
                case VSNLLineType.Label:
                    await UniTask.CompletedTask; 
                    break;

                case VSNLLineType.Command:
                    // Using CommandFactory (Assuming existing VSNL.Commands namespace works or needs using)
                    // We need to resolve VSNL.Commands if not in using.
                    var cmd = VSNL.Commands.CommandFactory.Create(line.CommandName);
                    if (cmd != null)
                    {
                        await cmd.ExecuteAsync(line.CommandArgs);
                    }
                    else
                    {
                        Debug.LogWarning($"[ScriptPlayer] Unknown command: {line.CommandName}");
                    }
                    await UniTask.CompletedTask;
                    break;

                case VSNLLineType.Dialogue:
                    var ui = Engine.Instance.GetService<UIManager>();
                    if (ui != null)
                    {
                        string formattedText = VSNL.Core.Utils.TextFormatter.FormatText(line.DialogText);
                        await ui.ShowDialogueAsync(line.Speaker, formattedText);
                    }
                    else
                    {
                        Debug.Log($"[DIALOGUE] {line.Speaker}: {line.DialogText}");
                    }
                    
                    // Wait for Input (Click)
                    var inputService = Engine.Instance.GetService<InputService>();
                    while (inputService != null && !inputService.SubmitAction.triggered)
                    {
                         if (_playCts.Token.IsCancellationRequested) return;
                         await UniTask.Yield();
                    }
                    await UniTask.DelayFrame(1);
                    break;
            }
        }

        #endregion
    }
}
