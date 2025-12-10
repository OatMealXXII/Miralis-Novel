using System;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using VSNL.Core;

namespace VSNL.State
{
    public class SaveLoadManager : MonoBehaviour, IGameService
    {
        public GameState CurrentState { get; private set; }

        private string _saveDirectory;

        public async UniTask InitializeAsync()
        {
            _saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }

            ResetService();
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            CurrentState = new GameState();
        }

        public async UniTask SaveGameAsync(int slotId)
        {
            // 1. Capture Screenshot
            string screenPath = Path.Combine(_saveDirectory, $"save_{slotId}.png");
            await CaptureScreenshotAsync(screenPath);

            // 2. Serialize State
            string jsonPath = Path.Combine(_saveDirectory, $"save_{slotId}.json");
            string json = JsonUtility.ToJson(CurrentState, true);
            await File.WriteAllTextAsync(jsonPath, json);
            
            Debug.Log($"[SaveLoadManager] Game Saved to Slot {slotId}");
        }

        public async UniTask LoadGameAsync(int slotId)
        {
            string path = Path.Combine(_saveDirectory, $"save_{slotId}.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveLoadManager] Save slot {slotId} not found.");
                return;
            }

            string json = await File.ReadAllTextAsync(path);
            var loadedState = JsonUtility.FromJson<GameState>(json);

            if (loadedState != null)
            {
                CurrentState = loadedState;
                await Engine.Instance.RestoreState(CurrentState);
                Debug.Log($"[SaveLoadManager] Game Loaded from Slot {slotId}");
            }
        }

        private async UniTask CaptureScreenshotAsync(string path)
        {
            // Must wait for end of frame
            await UniTask.WaitForEndOfFrame(this);
            
            // Capture
            Texture2D screenImage = new Texture2D(Screen.width, Screen.height);
            screenImage.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            screenImage.Apply();
            
            byte[] bytes = screenImage.EncodeToPNG();
            await File.WriteAllBytesAsync(path, bytes);
            
            Destroy(screenImage);
        }

        // Helper to update variables
        public void SetVariable(string key, string value)
        {
            int index = CurrentState.GlobalVariables.FindIndex(v => v.Key == key);
            if (index != -1)
            {
                var entry = CurrentState.GlobalVariables[index];
                entry.Value = value;
                CurrentState.GlobalVariables[index] = entry; 
            }
            else
            {
                CurrentState.GlobalVariables.Add(new VariableEntry { Key = key, Value = value });
            }
        }

        public string GetVariable(string key)
        {
             int index = CurrentState.GlobalVariables.FindIndex(v => v.Key == key);
             if (index != -1) return CurrentState.GlobalVariables[index].Value;
             return null;
        }
    }
}

