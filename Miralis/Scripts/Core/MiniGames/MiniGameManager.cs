using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using VSNL.Core;
using VSNL.UI;

namespace VSNL.Services
{
    public class MiniGameManager : MonoBehaviour, IGameService
    {
        private UniTaskCompletionSource<int> _miniGameTcs;
        private string _activeSceneName;

        public async UniTask InitializeAsync()
        {
            Debug.Log("[MiniGameManager] Initialized.");
            await UniTask.CompletedTask;
        }

        public void ResetService()
        {
            _miniGameTcs?.TrySetCanceled();
            _miniGameTcs = null;
        }

        public async UniTask PlayMiniGameAsync(string sceneName, string resultVarName)
        {
            if (_miniGameTcs != null)
            {
                Debug.LogWarning("[MiniGameManager] A mini-game is already active.");
                return;
            }

            _activeSceneName = sceneName;
            _miniGameTcs = new UniTaskCompletionSource<int>();

            // 1. Hide UI
            var uiManager = Engine.Instance.GetService<UIManager>();
            if (uiManager != null) uiManager.SetDialoguePanelVisibility(false);

            // 2. Load Scene Additively
            // Check if scene exists in build settings is hard at runtime without crashing if missing.
            // Assuming it exists.
            try
            {
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[MiniGameManager] Failed to load scene '{sceneName}': {e.Message}");
                if (uiManager != null) uiManager.SetDialoguePanelVisibility(true);
                _miniGameTcs = null;
                return;
            }

            Debug.Log($"[MiniGameManager] Started MiniGame: {sceneName}");

            // 3. Wait for Result
            int result = 0;
            try
            {
                result = await _miniGameTcs.Task;
            }
            catch (System.OperationCanceledException)
            {
                Debug.LogWarning("[MiniGameManager] MiniGame cancelled.");
            }

            // 4. Set Variable
            if (!string.IsNullOrEmpty(resultVarName))
            {
                var varMgr = Engine.Instance.GetService<VariableManager>();
                if (varMgr != null)
                {
                    // Ensure variable name starts with $? Command parser usually handles arguments, 
                    // but VariableManager usually expects name without $ for keys.
                    // Let's assume input is cleaned or raw. 
                    // Command_MiniGame should pass clean name.
                    varMgr.SetVariable(resultVarName, result);
                    Debug.Log($"[MiniGameManager] Set {resultVarName} = {result}");
                }
            }

            // 5. Unload Scene
            await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();

            // 6. Restore UI
            if (uiManager != null) uiManager.SetDialoguePanelVisibility(true);

            _miniGameTcs = null;
            _activeSceneName = null;
        }

        public void ReportResult(int score)
        {
            if (_miniGameTcs != null)
            {
                _miniGameTcs.TrySetResult(score);
            }
            else
            {
                Debug.LogWarning("[MiniGameManager] ReportResult called but no mini-game is active.");
            }
        }
    }
}
