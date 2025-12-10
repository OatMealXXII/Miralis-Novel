using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using VSNL.Commands;
using System.Linq;

using VSNL.Core;

namespace VSNL.DebugSystem
{
    public class DebugConsole : MonoBehaviour, IGameService
    {
        private DebugConsoleUI _ui;
        private bool _isVisible = false;
        private List<string> _commandHistory = new List<string>();
        private int _historyIndex = 0;

        public async UniTask InitializeAsync()
        {
            // Create UI if not exists
            var uiGo = new GameObject("DebugConsoleUI");
            _ui = uiGo.AddComponent<DebugConsoleUI>();
            uiGo.transform.SetParent(this.transform); // Keep it clean
            
            _ui.Initialize(this);
            _ui.SetVisible(false);
            
            Debug.Log("[DebugConsole] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService() { }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote)) // Tilde key
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            _isVisible = !_isVisible;
            _ui.SetVisible(_isVisible);
        }

        public async void ExecuteCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            Log($"> {input}");
            _commandHistory.Add(input);
            _historyIndex = _commandHistory.Count;

            string[] parts = input.Split(new[] { ' ' }, 2, System.StringSplitOptions.RemoveEmptyEntries);
            string cmdName = parts[0];
            string args = parts.Length > 1 ? parts[1] : "";

            var command = CommandFactory.Create(cmdName);
            if (command != null)
            {
                try
                {
                    await command.ExecuteAsync(args);
                    Log($"<color=green>Executed {cmdName}</color>");
                }
                catch (System.Exception ex)
                {
                    Log($"<color=red>Error: {ex.Message}</color>");
                }
            }
            else
            {
                Log($"<color=orange>Unknown command: {cmdName}</color>");
            }
        }

        public void Log(string message)
        {
            _ui.AppendLog(message);
        }

        public List<string> GetSuggestions(string partial)
        {
            if (string.IsNullOrWhiteSpace(partial)) return new List<string>();
            return CommandFactory.GetCommandNames()
                .Where(n => n.StartsWith(partial.ToLower()))
                .ToList();
        }
    }
}
