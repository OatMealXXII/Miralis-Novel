using System.Text.RegularExpressions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VSNL.State;

namespace VSNL.Core
{
    public class VariableManager : MonoBehaviour, IGameService
    {
        private SaveLoadManager _saveLoadManager;

        public async UniTask InitializeAsync()
        {
            _saveLoadManager = Engine.Instance.GetService<SaveLoadManager>();
            Debug.Log("[VariableManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            // Manager handles data reset
        }

        public void SetVariable(string name, string value)
        {
            if (!name.StartsWith("$")) name = "$" + name;
            _saveLoadManager.SetVariable(name, value);
        }

        public void SetVariable(string name, int value) 
        {
             SetVariable(name, value.ToString());
        }

        public string GetVariable(string name)
        {
            if (!name.StartsWith("$")) name = "$" + name;
            var val = _saveLoadManager.GetVariable(name);
            return val ?? ""; // Return empty string if not found
        }

        /// <summary>
        /// Replaces {$varName} with actual values in the text.
        /// </summary>
        public string ParseText(string text)
        {
            return Regex.Replace(text, @"\{\$([a-zA-Z0-9_]+)\}", match =>
            {
                string varName = match.Groups[1].Value;
                return GetVariable(varName);
            });
        }
    }
}

